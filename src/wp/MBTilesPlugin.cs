﻿/*
	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Security;
using System.Diagnostics;
using MBTilesPlugin;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class MBTilesPlugin : BaseCommand
    {
        private IMBTilesActions mbTilesActions = null;

        public static string ACTION_OPEN_TYPE_DB = "db";
	    public static string ACTION_OPEN_TYPE_FILE = "file";

        public void open(string options)
        {
            string callbackId;
            options = options.Replace("{}", ""); // empty objects screw up the Deserializer
            try
            {
                // name, type
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                // to test maybe is not a string but a JSONObject
                EntryOpen entryOpen = JSON.JsonHelper.Deserialize<EntryOpen>(args[0]);
                string name = entryOpen.name;
                string type = entryOpen.type;
                callbackId = args[1];

                if (type != null && type.Equals(ACTION_OPEN_TYPE_DB))
                {
                    string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, name);
                    mbTilesActions = new MBTilesActionsDatabaseImpl();
                    mbTilesActions.open(dbPath);
                   
                } else if (type != null && type.Equals(ACTION_OPEN_TYPE_FILE))
                {
                    string dirPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "maps//" + name);
                    mbTilesActions = new MBTilesActionsFileImpl();
                    mbTilesActions.open(dirPath);

                }

                if (mbTilesActions != null && mbTilesActions.isOpen())
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK), callbackId);
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.IO_EXCEPTION), callbackId);
                }

            }
            catch (Exception)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
            }

        }

        public void get_metadata(string options)
        {
            // no options
            string callbackId;
            options = options.Replace("{}", ""); // empty objects screw up the Deserializer
            try
            {
                // name, type
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                callbackId = args[0];

                if (mbTilesActions != null && mbTilesActions.isOpen())
                {
                    string metadata = mbTilesActions.getMetadata();
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, metadata), callbackId);
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.IO_EXCEPTION), callbackId);
                }
            }
            catch (Exception)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
            }
        }

        public void get_min_zoom(string options)
        {
            // no options
            string callbackId;
            options = options.Replace("{}", ""); // empty objects screw up the Deserializer
            try
            {
                // name, type
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                callbackId = args[0];

                if (mbTilesActions != null && mbTilesActions.isOpen())
                {
                    minzoom_output minzoom = mbTilesActions.getMinZoom();
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, minzoom), callbackId);
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.IO_EXCEPTION), callbackId);
                }

            }
            catch (Exception e)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
            }
        }

        public void get_max_zoom(string options)
        {
            // no options

            string callbackId;
            options = options.Replace("{}", ""); // empty objects screw up the Deserializer
            try
            {
                // name, type
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                callbackId = args[0];

                if (mbTilesActions != null && mbTilesActions.isOpen())
                {
                    maxzoom_output maxzoom = mbTilesActions.getMaxZoom();
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, maxzoom), callbackId);
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.IO_EXCEPTION), callbackId);
                }
            }
            catch (Exception)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
             }
        }

        public void get_tile(string options)
        {
             string callbackId;
            options = options.Replace("{}", ""); // empty objects screw up the Deserializer
            try
            {
                // z, x, y
                string[] args = JSON.JsonHelper.Deserialize<string[]>(options);
                // to test maybe is not an integer but a JSONObject
                EntryTile entryTile = JSON.JsonHelper.Deserialize<EntryTile>(args[0]);
                int z = entryTile.z;
                int x = entryTile.x;
                int y = entryTile.y;
                callbackId = args[1];

                if (mbTilesActions != null && mbTilesActions.isOpen())
                {
                    tiles_output tile = mbTilesActions.getTile(z,x,y);
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, tile), callbackId);
                }
                else
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.IO_EXCEPTION), callbackId);
                }

            }
            catch (Exception)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
            }
        }
    }
	
	[DataContract]
	public class EntryOpen 
	{
		[DataMember(Name = "name")]
		public string name{ get; set; }
		[DataMember(Name = "type")]
		public string type{ get; set; }
	}
	
	[DataContract]
	public class EntryTile 
	{
		[DataMember(Name = "z")]
		public int z{ get; set; }
		[DataMember(Name = "x")]
		public int x{ get; set; }
		[DataMember(Name = "y")]
		public int y{ get; set; }
	}
}