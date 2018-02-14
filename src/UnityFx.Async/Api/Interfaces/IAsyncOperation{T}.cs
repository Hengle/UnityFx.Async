﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Async
{
	/// <summary>
	/// Extends an <see cref="IAsyncOperation"/> interface with a result value.
	/// </summary>
	/// <seealso cref="IAsyncResult"/>
	/// <seealso cref="IAsyncOperation"/>
	public interface IAsyncOperation<out T> : IAsyncOperation
	{
		/// <summary>
		/// Gets the operation result value.
		/// </summary>
		/// <remarks>
		/// Once the result of an operation is available, it is stored and is returned immediately on subsequent calls to the <see cref="Result"/> property.
		/// Note that, if an exception occurred during the operation, or if the operation has been cancelled, the <see cref="Result"/> property does not return a value.
		/// Instead, attempting to access the property value throws an <see cref="InvalidOperationException"/> exception.
		/// </remarks>
		/// <value>Result of the operation.</value>
		/// <exception cref="InvalidOperationException">Thrown either if the property is accessed before operation is completed or if the operation has faulted.</exception>
		T Result { get; }
	}
}