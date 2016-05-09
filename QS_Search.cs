/* 
QuickSearch
Copyright 2016 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSearch {
	internal class QSearch : QuickSearch {

		internal enum partInfos {
			TAG,
			NAME,
			TITLE,
			DESCRIPTION,
			AUTHOR,
			MANUFACTURER,
			PARTSIZE,
			RESOURCEINFOS,
			TECHREQUIRED,
			MODULE,
			NONE
		}

		private static string text = string.Empty;
		public static string Text {
			get {
				return text;
			}
			set {
				text = CleanInput (value);
			}
		}
		private static string CleanInput(string strIn) {
			// Replace invalid characters with empty strings. 
			return Regex.Replace(strIn, @"[^\w%ù£¤'#~&`_²\{\}!\.@\-|&/\(\)\[\]\+?,;:/\*µ\^\$=\ ""]", string.Empty); 
		}

		// Retourner le filtre par fonctions
		internal static PartCategorizer.Category FilterByFunctions {
			get {
				return PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Function");
			}
		}

		private static bool partHasCategory(AvailablePart part) {
			if (part.category != PartCategories.none) {
				return true;
			}
			PartCategorizer.Category _filter = FilterByFunctions;
			List<PartCategorizer.Category> _subcategories = _filter.subcategories;
			bool _val = false;
			foreach (PartCategorizer.Category _subcategory in _subcategories) {
				if (_subcategory.exclusionFilter == null) {
					continue;
				}
				_val |= _subcategory.exclusionFilter.FilterCriteria.Invoke (part);
			}
			return _val;
		}

		private static string PartInfo(AvailablePart part, string search) {
			string _partinfo = " ";
			if (!string.IsNullOrEmpty(part.tags) && searchExtension(partInfos.TAG, search)) {
				_partinfo += part.tags + " ";
			}
			if (part.title != null && searchExtension(partInfos.TITLE, search)) {
				_partinfo += part.title + " ";
			}
			if (part.author != null && searchExtension(partInfos.AUTHOR, search)) {
				_partinfo += part.author + " ";
			}
			if (part.manufacturer != null && searchExtension(partInfos.MANUFACTURER, search)) {
				_partinfo += part.manufacturer + " ";
			}
			if (part.name != null && searchExtension(partInfos.NAME, search)) {
				_partinfo += part.name + " ";
			}
			if (!float.IsNaN(part.partSize) && searchExtension(partInfos.PARTSIZE, search)) {
				_partinfo += part.partSize + " ";
			}

			if (part.resourceInfos.Count > 0 && searchExtension(partInfos.RESOURCEINFOS, search)) {
				_partinfo += part.resourceInfo + " ";
				List<AvailablePart.ResourceInfo> _resourceInfos = part.resourceInfos;
				foreach (AvailablePart.ResourceInfo _resourceInfo in _resourceInfos) {
					_partinfo += _resourceInfo.resourceName + " ";
				}
			}
			if (part.TechRequired != null && searchExtension(partInfos.TECHREQUIRED, search)) {
				_partinfo += part.TechRequired + " ";
			}
			if (part.moduleInfos.Count > 0 && searchExtension(partInfos.MODULE, search)) {
				List<AvailablePart.ModuleInfo> _moduleInfos = part.moduleInfos;
				foreach (AvailablePart.ModuleInfo _moduleInfo in _moduleInfos) {
					_partinfo += _moduleInfo.moduleName + " ";
				}
			}
			if (part.description != null && searchExtension(partInfos.DESCRIPTION, search)) {
				_partinfo += part.description;
			}
			return _partinfo;
		}

		private static string ShipInfo(ShipTemplate ship) {
			string _shipinfo = string.Empty;
			if (ship.shipName != null) {
				_shipinfo += ship.shipName + " ";
			}
			if (ship.shipDescription != null) {
				_shipinfo += ship.shipDescription + " ";
			}
			return _shipinfo;
		}

		internal static bool FindPart(AvailablePart part) {
			if (part == null) {
				return false;
			}
			if (Text == string.Empty) {
				return true;
			}
			string _partinfo = PartInfo (part, Text);
			if (_partinfo == string.Empty) {
				return false;
			}
			string _Text = Text;
			if (QSettings.Instance.enableSearchExtension && _Text.StartsWith(QSettings.Instance.searchRegex) && _Text.EndsWith((string)QSettings.Instance.searchRegex)) {
				try {
					_Text = _Text.Substring(1, _Text.Length -2);
					return Regex.IsMatch (_partinfo, _Text);
				} catch {
					return FindStandard (_partinfo, _Text);
				}
			} else {
				return FindStandard (_partinfo, Text);
			}
		}

		internal static bool FindStandard(string Infos, string search) {
			if (!QSettings.Instance.enableSearchExtension) {
				return Infos.Contains (search);
			}
			bool _Return = false;
			string[] _OrSplits = search.ToLower ().Split (QSettings.Instance.searchOR.ToCharArray (0, 1));
			Infos = Infos.ToLower ();
			foreach (string _OrSplit in _OrSplits) {
				string[] _AndSplits = _OrSplit.Split(QSettings.Instance.searchAND.ToCharArray (0, 1));
				if (_AndSplits.Length > 1) {
					bool _AndReturn = true;
					foreach (string _AndSplit in _AndSplits) {
						if (!_AndSplit.StartsWith (QSettings.Instance.searchNOT)) {
							_AndReturn = _AndReturn && Infos.Contains (searchExtension (_AndSplit));
						} else {
							_AndReturn = _AndReturn && !Infos.Contains (searchExtension (_AndSplit));
						}
					}
					_Return = _Return || _AndReturn;
					if (_Return) {
						break;
					}
				} else {
					if (!_OrSplit.StartsWith (QSettings.Instance.searchNOT)) {
						if (Infos.Contains (searchExtension (_OrSplit))) {
							_Return = true;
							break;
						}
					} else {
						if (!Infos.Contains (searchExtension (_OrSplit))) {
							_Return = true;
							break;
						}
					}
				}
			}
			return _Return;
		}

		private static string searchExtension(string search) {
			if (QSettings.Instance.enableSearchExtension && search.Length > 1) {
				if ((StartsWith (search, QSettings.Instance.searchNOT)) || 
					(StartsWith (search, QSettings.Instance.searchTag)) || 
					(StartsWith (search, QSettings.Instance.searchName)) || 
					(StartsWith (search, QSettings.Instance.searchTitle)) || 
					(StartsWith (search, QSettings.Instance.searchDescription)) || 
					(StartsWith (search, QSettings.Instance.searchAuthor)) || 
					(StartsWith (search, QSettings.Instance.searchManufacturer)) || 
					(StartsWith (search, QSettings.Instance.searchPartSize)) || 
					(StartsWith (search, QSettings.Instance.searchResourceInfos)) || 
					(StartsWith (search, QSettings.Instance.searchTechRequired)) || 
					(StartsWith (search, QSettings.Instance.searchModule))) {
					search = search.Remove (0,1);
				}
				if (StartsWith (search, QSettings.Instance.searchBegin) || StartsWith (search, QSettings.Instance.searchWord)) {
					search = " " + search.Remove (0,1);
				}
				if (EndsWith (search, QSettings.Instance.searchEnd) || EndsWith (search, QSettings.Instance.searchWord)) {
					search = search.Remove (search.Length - 1) + " ";
				}
			}
			return search;
		}

		private static bool StartsWith(string search, string pre) {
			return !string.IsNullOrEmpty(pre) && search.StartsWith (pre);
		}

		private static bool EndsWith(string search, string pre) {
			return !string.IsNullOrEmpty(pre) && search.EndsWith (pre);
		}
		private static bool searchExtension(partInfos pInfo, string search) {
			if (QSettings.Instance.enableSearchExtension && search.Length > 1) {
				if (search.StartsWith (QSettings.Instance.searchTag)) {
					if (pInfo == partInfos.TAG) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchName)) {
					if (pInfo == partInfos.NAME) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchTitle)) {
					if (pInfo == partInfos.TITLE) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchDescription)) {
					if (pInfo == partInfos.DESCRIPTION) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchAuthor)) {
					if (pInfo == partInfos.AUTHOR) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchManufacturer)) {
					if (pInfo == partInfos.MANUFACTURER) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchPartSize)) {
					if (pInfo == partInfos.PARTSIZE) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchResourceInfos)) {
					if (pInfo == partInfos.RESOURCEINFOS) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchTechRequired)) {
					if (pInfo == partInfos.TECHREQUIRED) {
						return true;
					}
					return false;
				}
				if (search.StartsWith (QSettings.Instance.searchModule)) {
					if (pInfo == partInfos.MODULE) {
						return true;
					}
					return false;
				}
			}
			return true;
		}

		internal static bool FindSubassembly(ShipTemplate ship) {
			if (ship == null) {
				return false;
			}
			if (Text == string.Empty) {
				return true;
			}
			string _shipinfo = ShipInfo (ship);
			if (_shipinfo == string.Empty) {
				return false;
			}
			string _Text = Regex.Replace (Text, @"^/([^/]+)/$", "$1");
			if (_Text != Text) {
				try {
					return Regex.IsMatch (_shipinfo, _Text);
				} catch {
					return FindStandard (_shipinfo, _Text);
				}
			} else {
				return FindStandard (_shipinfo, Text);
			}
		}
	}
}