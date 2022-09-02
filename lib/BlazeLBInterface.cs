﻿using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Discord;

namespace BlazeDiscordRPC {
	public static class BlazeLBInterface {

		public static IEnumerator UpdateStatus(Activity activity, string online) {
			WWWForm form = new WWWForm();
			form.AddField("steamid", KKMGLMJABKH.NBDMKLFDACD.ToString());
            form.AddField("online", online);
            form.AddField("state", activity.State != null ? activity.State : "");
			form.AddField("details", activity.Details != null ? activity.Details : "");

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/api/users/llb-status", form)) {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError) BlazeDiscordRPC.Log.LogError(www.error);
            }
		}

	}
}
