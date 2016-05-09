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
using System.IO;
using UnityEngine;

namespace QuickSearch {
	public class QSettings : QuickSearch {

		public readonly static QSettings Instance = new QSettings();

		private string FileConfig = KSPUtil.ApplicationRootPath + "GameData/" + MOD + "/Config.txt";

		public readonly static ulong idSearchKey = 1584651;

		[Persistent] internal bool Debug = false;

		[Persistent] internal bool StockToolBar = true;
		[Persistent] internal bool BlizzyToolBar = true;

		[Persistent] internal bool EditorSearch = true;
		[Persistent] internal bool RnDSearch = true;
		[Persistent] internal bool enableSearchExtension = true;

		[Persistent] internal string searchAND = 			"&";
		[Persistent] internal string searchOR = 			"|";
		[Persistent] internal string searchNOT = 			"!";
		[Persistent] internal string searchWord = 			"\"";
		[Persistent] internal string searchBegin = 			"(";
		[Persistent] internal string searchEnd = 			")";
		[Persistent] internal string searchRegex = 			"/";
		[Persistent] internal string searchTag = 			"%";
		[Persistent] internal string searchName = 			";";
		[Persistent] internal string searchTitle = 			":";
		[Persistent] internal string searchDescription = 	"-";
		[Persistent] internal string searchAuthor = 		",";
		[Persistent] internal string searchManufacturer = 	"?";
		[Persistent] internal string searchPartSize = 		".";
		[Persistent] internal string searchResourceInfos = 	"+";
		[Persistent] internal string searchTechRequired = 	"@";
		[Persistent] internal string searchModule = 		"_";

		// GESTION DE LA CONFIGURATION
		public void Save() {
			ConfigNode _temp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
			_temp.Save(FileConfig);
			Log ("Settings Saved", "QSettings");
		}
		public void Load() {
			if (File.Exists (FileConfig)) {
				try {
					ConfigNode _temp = ConfigNode.Load (FileConfig);
					ConfigNode.LoadObjectFromConfig (this, _temp);
				} catch {
					Save ();
				}
				Log ("Settings Loaded", "QSettings");
			} else {
				Save ();
			}
		}
	}
}