﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using UnityEngine;
#if UNITY_2018_2_OR_NEWER
using UnityEngine.Video;
#endif

namespace UnityFx.Async
{
	/// <summary>
	/// A wrapper for <see cref="WWW"/> with result value.
	/// </summary>
	/// <typeparam name="T">Type of the request result.</typeparam>
	public class WwwResult<T> : AsyncResult<T> where T : class
	{
		#region data

		private readonly WWW _www;

		#endregion

		#region interface

		/// <summary>
		/// Gets the underlying <see cref="WWW"/> instance.
		/// </summary>
		public WWW WebRequest
		{
			get
			{
				return _www;
			}
		}

		/// <summary>
		/// Gets the request url string.
		/// </summary>
		public string Url
		{
			get
			{
				return _www.url;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WwwResult{T}"/> class.
		/// </summary>
		/// <param name="request">Source web request.</param>
		public WwwResult(WWW request)
			: this(request, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WwwResult{T}"/> class.
		/// </summary>
		/// <param name="request">Source web request.</param>
		/// <param name="userState">User-defined data.</param>
		public WwwResult(WWW request, object userState)
			: base(null, userState)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			_www = request;
		}

		/// <summary>
		/// Initializes the operation result value. Called when the underlying <see cref="WWW"/> has completed withou errors.
		/// </summary>
		protected virtual T GetResult(WWW request)
		{
			if (typeof(T) == typeof(AssetBundle))
			{
				return request.assetBundle as T;
			}
			else if (typeof(T) == typeof(Texture2D))
			{
				return request.texture as T;
			}
			else if (typeof(T) == typeof(AudioClip))
			{
#if UNITY_5_4_OR_NEWER
				return request.GetAudioClip() as T;
#else
				return request.audioClip as T;
#endif
			}
#if UNITY_2018_2_OR_NEWER
			else if (typeof(T) == typeof(VideoClip))
			{
				// TODO
				throw new NotImplementedException();
			}
#else
			else if (typeof(T) == typeof(MovieTexture))
			{
#if UNITY_5_4_OR_NEWER
				return request.GetMovieTexture() as T;
#else
				return request.movie as T;
#endif
			}
#endif
			else if (typeof(T) == typeof(byte[]))
			{
				return request.bytes as T;
			}
			else if (typeof(T) != typeof(object))
			{
				return request.text as T;
			}

			return null;
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override float GetProgress()
		{
			return _www.progress;
		}

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			if (_www.isDone)
			{
				SetCompleted();
			}
			else
			{
				AsyncUtility.AddCompletionCallback(_www, SetCompleted);
			}
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_www.Dispose();
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			var result = new StringBuilder();
			var errorStr = _www.error;

			result.Append(_www.GetType().Name);
			result.Append(" (");
			result.Append(_www.url);

			if (IsFaulted && !string.IsNullOrEmpty(errorStr))
			{
				result.Append(", ");
				result.Append(errorStr);
			}

			result.Append(')');
			return result.ToString();
		}

		#endregion

		#region implementation

		private void SetCompleted()
		{
			try
			{
				if (string.IsNullOrEmpty(_www.error))
				{
					var result = GetResult(_www);
					TrySetResult(result);
				}
				else
				{
					TrySetException(new WebRequestException(_www.error));
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		#endregion
	}
}
