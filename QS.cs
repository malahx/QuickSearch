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

namespace QuickSearch {
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class QuickSearch : Quick {

		private void Awake() {
			GameEvents.onGUIEditorToolbarReady.Add (OnGUIEditorToolbarReady);
		}

		private void Start() {
			QPersistent.Init ();
			QGUI.Init ();
		}

		private void OnGUIEditorToolbarReady() {
			QCategory.Init ();
		}

		private void OnGUI() {
			if (HighLogic.LoadedSceneIsEditor) {
				if (EditorLogic.fetch.editorScreen == EditorScreen.Parts && PartCategorizer.Ready && EditorPanels.Instance != null) {
					if (QCategory.Ready && QGUI.Ready) {
						QCategory.OnGUI ();
						QGUI.OnGUI ();
					} else {
						if (!QCategory.Ready) {
							QCategory.Init ();
						}
						if (!QGUI.Ready) {
							QGUI.Init ();
						}
					}
				}
			}
		}

		private void OnDestroy() {
			GameEvents.onGUIEditorToolbarReady.Remove (OnGUIEditorToolbarReady);
		}
	}
}