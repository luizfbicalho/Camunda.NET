using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.variable.value.builder
{

	/// <summary>
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// 
	/// </summary>
	public interface FileValueBuilder : TypedValueBuilder<FileValue>
	{

	  /// <summary>
	  /// Saves the MIME type of a file in the value infos.
	  /// </summary>
	  /// <param name="mimeType">
	  ///          the MIME type as string </param>
	  FileValueBuilder mimeType(string mimeType);

	  /// <summary>
	  /// Sets the value to the specified <seealso cref="File"/>.
	  /// </summary>
	  /// <seealso cref= #file(byte[]) </seealso>
	  /// <seealso cref= #file(InputStream) </seealso>
	  FileValueBuilder file(File file);

	  /// <summary>
	  /// Sets the value to the specified <seealso cref="InputStream"/>.
	  /// </summary>
	  /// <seealso cref= #file(byte[]) </seealso>
	  /// <seealso cref= #file(File) </seealso>
	  FileValueBuilder file(Stream stream);

	  /// <summary>
	  /// Sets the value to the specified <seealso cref="Byte"/> array
	  /// </summary>
	  /// <seealso cref= #file(File) </seealso>
	  /// <seealso cref= #file(InputStream) </seealso>
	  FileValueBuilder file(sbyte[] bytes);

	  /// <summary>
	  /// Sets the encoding for the file in the value infos (optional).
	  /// </summary>
	  /// <param name="encoding">
	  /// @return </param>
	  FileValueBuilder encoding(Charset encoding);

	  /// <summary>
	  /// Sets the encoding for the file in the value infos (optional).
	  /// </summary>
	  /// <param name="encoding">
	  /// @return </param>
	  FileValueBuilder encoding(string encoding);

	}

}