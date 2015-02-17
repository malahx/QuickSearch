/* 
QuickSearch
Copyright 2015 Malah

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
using UnityEngine;

namespace QuickSearch {

	internal class QPersistent : MonoBehaviour {

		private static string File_settings = KSPUtil.ApplicationRootPath + "GameData/" + Quick.MOD + "/Persistent.txt";

		private static ConfigNode Persistent = new ConfigNode();

		internal static List<string> GetItem {
			get {
				List<string> _stringnodes = new List<string> ();
				ConfigNode[] _nodes = Persistent.GetNodes ();
				foreach (ConfigNode _node in _nodes) {
					_stringnodes.Add (_node.name);
				}
				return _stringnodes;
			}
		}

		internal static string GetDisplayType(string item) {
			return Persistent.GetNode (item).GetValue ("displayType");
		}

		internal static Color GetColor(string item) {
			ConfigNode color = Persistent.GetNode (item).GetNode("color");
			float _r = float.Parse (color.GetValue ("r"));
			float _g = float.Parse (color.GetValue ("g"));
			float _b = float.Parse (color.GetValue ("b"));
			return new Color (_r, _g, _b);
		}

		internal static void Init() {
			Load ();
		}

		internal static void Add(string item, EditorPartList.State displayType, Color color) {
			if (Persistent.HasNode ("item")) {
				return;
			}
			ConfigNode _node = Persistent.AddNode (item);
			_node.AddValue("displayType", displayType);
			_node = _node.AddNode ("color");
			_node.AddValue ("r", color.r);
			_node.AddValue ("g", color.g);
			_node.AddValue ("b", color.b);
			Save ();
		}

		internal static void Remove(string item) {
			Persistent.RemoveNode (item);
			Save ();
		}

		private static void Save() {
			if (Persistent.CountNodes > 0) {
				Persistent.Save (File_settings);
			} else if (File.Exists (File_settings)) {
				File.Delete (File_settings);
			}
			Quick.Log ("Persistent Save");
		}

		private static void Load() {
			if (File.Exists (File_settings)) {
				Persistent = ConfigNode.Load (File_settings);
				Quick.Log ("Persistent Load");
			}
		}
	}
}