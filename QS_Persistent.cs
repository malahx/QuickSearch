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
using System.IO;
using RUI.Icons.Selectable;
using UnityEngine;

namespace QuickSearch {

	internal class QPersistent {

		private static string File_settings = KSPUtil.ApplicationRootPath + "GameData/" + QuickSearch.MOD + "/Persistent.txt";

		private static ConfigNode Persistent = new ConfigNode();

		private static ConfigNode FindThisSearch(string name) {
			ConfigNode[] _nodes = Persistent.GetNodes ("SEARCH");
			foreach (ConfigNode _node in _nodes) {
				if (_node.HasValue ("name")) {
					if (_node.GetValue ("name") == name) {
						return _node;
					}
				}
			}
			return new ConfigNode ();
		}

		internal static List<string> GetItems {
			get {
				List<string> _stringnodes = new List<string> ();
				ConfigNode[] _nodes = Persistent.GetNodes ("SEARCH");
				foreach (ConfigNode _node in _nodes) {
					if (_node.HasValue ("name")) {
						_stringnodes.Add (_node.GetValue ("name"));
					}
				}
				return _stringnodes;
			}
		}

		internal static string GetDisplayType(string item) {
			return FindThisSearch (item).GetValue ("displayType");
		}

		internal static Color GetColor(string item) {
			ConfigNode color = FindThisSearch (item).GetNode("color");
			float _r = float.Parse (color.GetValue ("r"));
			float _g = float.Parse (color.GetValue ("g"));
			float _b = float.Parse (color.GetValue ("b"));
			return new Color (_r, _g, _b);
		}

		internal static Icon GetIcon(string item) {
			ConfigNode _nodes = FindThisSearch (item);
			if (!_nodes.HasValue("icon")) {
				return QCategory.Icon;
			}
			string _iconName = _nodes.GetValue("icon");
			Icon _icon;
			if (PartCategorizer.Instance.iconLoader.iconDictionary.TryGetValue(_iconName, out _icon)) {
				return _icon;
			}
			return QCategory.Icon;
		}

		internal static void Init() {
			Load ();
		}

		internal static void Add(string item, EditorPartList.State displayType, Color color, string icon = "") {
			if (Persistent.HasNode ("item")) {
				return;
			}
			ConfigNode _node = Persistent.AddNode ("SEARCH");
			_node.AddValue ("name", item);
			_node.AddValue("displayType", displayType);
			if (icon != string.Empty) {
				_node.AddValue ("icon", icon);
			}
			_node = _node.AddNode ("color");
			_node.AddValue ("r", color.r);
			_node.AddValue ("g", color.g);
			_node.AddValue ("b", color.b);
			Save ();
		}

		internal static void Remove(string name) {
			ConfigNode[] _nodes = Persistent.GetNodes ("SEARCH");
			ConfigNode _persistent = new ConfigNode ();
			foreach (ConfigNode _node in _nodes) {
				if (_node.HasValue ("name")) {
					if (_node.GetValue ("name") != name) {
						_persistent.AddNode (_node);
					}
				}
			}
			Persistent = _persistent;
			Save ();
		}

		private static void Save() {
			if (Persistent.GetNodes("SEARCH").Length > 0) {
				Persistent.Save (File_settings);
			} else if (File.Exists (File_settings)) {
				File.Delete (File_settings);
			}
			QuickSearch.Log ("Persistent Save");
		}

		private static void Load() {
			if (File.Exists (File_settings)) {
				try {
					Persistent = ConfigNode.Load (File_settings);
					QuickSearch.Log ("Persistent Load");
				}
				catch {
					Save ();
				}
			}
		}
	}
}