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
namespace org.camunda.bpm.engine.variable.type
{
	/// <summary>
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// </summary>
	public interface FileValueType : ValueType
	{

	  /// <summary>
	  /// Identifies the file's name as specified on value creation.
	  /// </summary>

	  /// <summary>
	  /// Identifies the file's mime type as specified on value creation.
	  /// </summary>

	  /// <summary>
	  /// Identifies the file's encoding as specified on value creation.
	  /// </summary>

	}

	public static class FileValueType_Fields
	{
	  public const string VALUE_INFO_FILE_NAME = "filename";
	  public const string VALUE_INFO_FILE_MIME_TYPE = "mimeType";
	  public const string VALUE_INFO_FILE_ENCODING = "encoding";
	}

}