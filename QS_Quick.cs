﻿/* 
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
using UnityEngine;

namespace QuickSearch {
	public class Quick : MonoBehaviour {

		public readonly static string VERSION = "1.00";
		public readonly static string MOD = "QuickSearch";
		private static bool isdebug = true;
		internal static void Log(string msg) {
			if (isdebug) {
				Debug.Log (MOD + "(" + VERSION + "): " + msg);
			}
		}
		internal static void Warning(string msg) {
			if (isdebug) {
				Debug.LogWarning (MOD + "(" + VERSION + "): " + msg);
			}
		}
	}
}