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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace QuickSearch2 {
	public partial class QuickSearch2 {
		
		private GUIStyle TextField;
		private GUIStyle ButtonStyle;

		internal bool Ready = false;

		private string DeleteTexturePath = "QuickSearch/Textures/delete";
		private Texture2D DeleteTexture;

		private Rect RectRDSearch {
			get {
				return new Rect (Screen.width / 2 - 200, Screen.height - 50, 200, 40);
			}
		}			

		internal void Init() {
			TextField = new GUIStyle(HighLogic.Skin.textField);
			TextField.stretchWidth = true;
			TextField.fixedHeight = 20;
			TextField.alignment = TextAnchor.MiddleCenter;
			ButtonStyle = new GUIStyle(HighLogic.Skin.button);
			ButtonStyle.alignment = TextAnchor.MiddleCenter;
			ButtonStyle.padding = new RectOffset (0, 0, 0, 0);
			ButtonStyle.imagePosition = ImagePosition.ImageOnly;
			DeleteTexture = GameDatabase.Instance.GetTexture (DeleteTexturePath, false);
			Log ("GUI Init", true);
		}

		internal void OnGUI() {
			if (!Ready) {
				return;
			}
			GUI.skin = HighLogic.Skin;
			GUILayout.BeginArea (RectRDSearch);
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			string _Text = GUILayout.TextField (QSearch.Text, TextField);
			if (GUILayout.Button (new GUIContent (DeleteTexture, "Clear the search bar"), ButtonStyle, GUILayout.Width (20), GUILayout.Height (20))) {
				_Text = string.Empty;
			}
			_Text = CleanInput(_Text);
			if (_Text != QSearch.Text) {
				QSearch.Text = _Text;
				if (_Text == string.Empty) {
					QSearchRD.Find (true);
				} else {
					QSearchRD.Find ();
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
		}

		private string CleanInput(string strIn) {
			// Replace invalid characters with empty strings. 
			return Regex.Replace(strIn, @"[^\w\.@-|&/\(\)\[\]\+?,;:/\*µ\^\$=\ ""]", string.Empty); 
		}
	}
}