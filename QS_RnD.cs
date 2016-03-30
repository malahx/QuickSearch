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
using KSP.UI;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSearch {
	public partial class QRnD {

		public static QRnD Instance;

		private GUIStyle TextField;
		private GUIStyle ButtonStyle;

		public bool Ready = false;

		private string DeleteTexturePath = "QuickSearch/Textures/delete";
		private Texture2D DeleteTexture;

		private Rect RectRDSearch {
			get {
				return new Rect (Screen.width - 250, Screen.height - 50, 200, 40);
			}
		}

		protected override void Awake() {
			if (HighLogic.LoadedScene != GameScenes.SPACECENTER) {
				Warning ("This mod works only on the SpaceCenter. Destroy.");
				Destroy (this);
				return;
			}
			if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER && HighLogic.CurrentGame.Mode != Game.Modes.SCIENCE_SANDBOX) {
				Warning ("This mod works only on a Career or on a Science gamemode. Destroy.");
				Destroy (this);
				return;
			}
			if (Instance != null) {
				Warning ("There's already an Instance of " + MOD +". Destroy.");
				Destroy (this);
				return;
			}
			Instance = this;
			Log ("Awake", "QRnD");
		}

		protected override void Start() {
			TextField = new GUIStyle(HighLogic.Skin.textField);
			TextField.stretchWidth = true;

			TextField.alignment = TextAnchor.MiddleCenter;
			ButtonStyle = new GUIStyle(HighLogic.Skin.button);
			ButtonStyle.alignment = TextAnchor.MiddleCenter;
			ButtonStyle.padding = new RectOffset (0, 0, 0, 0);
			ButtonStyle.imagePosition = ImagePosition.ImageOnly;
			DeleteTexture = GameDatabase.Instance.GetTexture (DeleteTexturePath, false);
			GameEvents.onGUIRnDComplexSpawn.Add (RnDComplexSpawn);
			GameEvents.onGUIRnDComplexDespawn.Add (RnDComplexDespawn);
			Log ("Start", "QRnD");
		}

		private void RnDComplexSpawn() {
			Ready = true;
			QSearch.Text = string.Empty;
			Log ("RnDComplexSpawn", "QRnD");
		}

		private void RnDComplexDespawn() {
			Ready = false;
			QSearch.Text = string.Empty;			
			Log ("RnDComplexDespawn", "QRnD");
		}

		protected override void OnDestroy() {
			GameEvents.onGUIRnDComplexSpawn.Remove (RnDComplexSpawn);
			GameEvents.onGUIRnDComplexDespawn.Remove (RnDComplexDespawn);
			Log ("OnDestroy", "QRnD");
		}

		internal void OnGUI() {
			if (!Ready) {
				return;
			}
			GUI.skin = HighLogic.Skin;
			GUILayout.BeginArea (RectRDSearch);
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			string _Text = GUILayout.TextField (QSearch.Text, TextField,GUILayout.Height(20));
			if (GUILayout.Button (new GUIContent (DeleteTexture, "Clear the search bar"), ButtonStyle,GUILayout.Height(20),GUILayout.Width(20))) {
				_Text = string.Empty;
			}
			_Text = CleanInput(_Text);
			if (_Text != QSearch.Text) {
				QSearch.Text = _Text;
				if (_Text == string.Empty) {
					Find (true);
				} else {
					Find ();
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

		internal static void Find(bool clean = false) {
			List<RDNode> _nodes = RDController.Instance.nodes;
			foreach (RDNode _node in _nodes) {
				RDTech _rdTech = _node.tech;
				UIStateButton _button = _node.graphics.button;
				if (!clean && _rdTech.partsAssigned.Find (aPart => QSearch.FindPart (aPart)) != null) {
					_button.Image.color = new Color (1f, 0f, 0f);
					continue;
				}
				_button.Image.color = new Color (1f, 1f, 1f);
			}
			Log ("Find: " + QSearch.Text, "QRnD");
		}
	}
}