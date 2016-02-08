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

namespace QuickSearch2 {
	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public partial class QuickSearch2 {

		public static QuickSearch2 Instance;

		private void Awake() {
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
			GameEvents.onGUIRnDComplexSpawn.Add (RnDComplexSpawn);
			GameEvents.onGUIRnDComplexDespawn.Add (RnDComplexDespawn);
			Log ("Awake", true);
		}

		private void Start() {
			Init ();
			Log ("Start", true);
		}

		private void RnDComplexSpawn() {
			Ready = true;
			QSearch.Text = string.Empty;
			Log ("RnDComplexSpawn", true);
		}
		private void RnDComplexDespawn() {
			Ready = false;
			QSearch.Text = string.Empty;			
			Log ("RnDComplexDespawn", true);
		}

		private void OnDestroy() {
			GameEvents.onGUIRnDComplexSpawn.Remove (RnDComplexSpawn);
			GameEvents.onGUIRnDComplexDespawn.Remove (RnDComplexDespawn);
			Log ("OnDestroy", true);
		}
	}
}