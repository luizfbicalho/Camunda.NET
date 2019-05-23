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
namespace org.camunda.bpm.engine.variable.value
{

	/// <summary>
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// 
	/// </summary>
	public interface FileValue : TypedValue
	{

	  string Filename {get;}

	  string MimeType {get;}

	  /// <summary>
	  /// Convenience method to save the transformation. This method will perform no
	  /// check if the saved encoding is known to the JVM and therefore could throw
	  /// every exception that <seealso cref="Charset.forName(string)"/> lists.
	  /// <para>
	  /// If no encoding has been saved it will return null.
	  /// 
	  /// </para>
	  /// </summary>
	  Charset EncodingAsCharset {get;}

	  /// <returns> the saved encoding or null if none has been saved </returns>
	  string Encoding {get;}

	  Stream Value {get;}

	}

}