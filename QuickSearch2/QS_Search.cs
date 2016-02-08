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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace QuickSearch2 {
	internal class QSearch {

		internal static string Text = string.Empty;

		private static string PartInfo(AvailablePart part) {
			string _partinfo = string.Empty;
			if (part.title != null) {
				_partinfo += part.title + " ";
			}
			if (part.author != null) {
				_partinfo += part.author + " ";
			}
			if (part.manufacturer != null) {
				_partinfo += part.manufacturer + " ";
			}
			if (part.name != null) {
				_partinfo += part.name + " ";
			}
			if (!float.IsNaN(part.partSize)) {
				_partinfo += part.partSize + " ";
			}

			if (part.resourceInfos.Count > 0) {
				_partinfo += part.resourceInfo + " ";
				List<AvailablePart.ResourceInfo> _resourceInfos = part.resourceInfos;
				foreach (AvailablePart.ResourceInfo _resourceInfo in _resourceInfos) {
					_partinfo += _resourceInfo.resourceName + " ";
				}
			}
			if (part.TechRequired != null) {
				_partinfo += part.TechRequired + " ";
			}
			if (part.moduleInfos.Count > 0) {
				List<AvailablePart.ModuleInfo> _moduleInfos = part.moduleInfos;
				foreach (AvailablePart.ModuleInfo _moduleInfo in _moduleInfos) {
					_partinfo += _moduleInfo.moduleName + " ";
				}
			}
			if (part.description != null) {
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
			string _partinfo = PartInfo (part);
			if (_partinfo == string.Empty) {
				return false;
			}
			string _Text = Regex.Replace (Text, @"^/([^/]+)/$", "$1");

			if (_Text != Text) {
				try {
					return Regex.IsMatch (_partinfo, _Text);
				} catch {
					return FindStandard (_partinfo, _Text);
				}
			} else {
				return FindStandard (_partinfo, Text);
			}
		}

		internal static bool FindStandard(string Infos, string Search) {
			bool _Return = false;
			string[] _OrSplits = Search.ToLower().Split('|');
			Infos = Infos.ToLower ();
			foreach (string _OrSplit in _OrSplits) {
				string[] _AndSplits = _OrSplit.Split('&');
				if (_AndSplits.Length > 1) {
					bool _AndReturn = true;
					foreach (string _AndSplit in _AndSplits) {
						_AndReturn = _AndReturn && Infos.Contains (_AndSplit);
					}
					_Return = _Return || _AndReturn;
					if (_Return) {
						break;
					}
				} else {
					if (Infos.Contains (_OrSplit)) {
						_Return = true;
						break;
					}
				}
			}
			return _Return;
		}
	}
}