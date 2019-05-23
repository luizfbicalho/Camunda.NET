using System.Collections.Generic;
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
namespace org.camunda.bpm.engine.rest.mapper
{
	using FileItemStream = org.apache.commons.fileupload.FileItemStream;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;


	/// <summary>
	/// Custom implementation of Multipart Form Data which can be used for handling requests.
	/// <para>
	/// Provides access to the form parts via <seealso cref="getNamedPart(string)"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public class MultipartFormData
	{

	  protected internal IDictionary<string, FormPart> formParts = new Dictionary<string, FormPart>();

	  public virtual void addPart(FormPart formPart)
	  {
		formParts[formPart.FieldName] = formPart;
	  }

	  public virtual FormPart getNamedPart(string name)
	  {
		return formParts[name];
	  }

	  public virtual ISet<string> PartNames
	  {
		  get
		  {
			return formParts.Keys;
		  }
	  }

	  /// <summary>
	  /// Dto representing a part in a multipart form.
	  /// 
	  /// </summary>
	  public class FormPart
	  {

		protected internal string fieldName;
		protected internal string contentType;
		protected internal string textContent;
		protected internal string fileName;
		protected internal sbyte[] binaryContent;

		public FormPart(FileItemStream stream)
		{
		  fieldName = stream.FieldName;
		  contentType = stream.ContentType;
		  binaryContent = readBinaryContent(stream);
		  fileName = stream.Name;

		  if (string.ReferenceEquals(contentType, null) || contentType.Contains(MediaType.TEXT_PLAIN))
		  {
			textContent = StringHelper.NewString(binaryContent);
		  }
		}

		public FormPart()
		{
		}

		protected internal virtual sbyte[] readBinaryContent(FileItemStream stream)
		{
		  Stream inputStream = getInputStream(stream);
		  return IoUtil.readInputStream(inputStream, stream.FieldName);
		}

		protected internal virtual Stream getInputStream(FileItemStream stream)
		{
		  try
		  {
			return stream.openStream();
		  }
		  catch (IOException e)
		  {
			throw new RestException(Status.INTERNAL_SERVER_ERROR, e);
		  }
		}

		public virtual string FieldName
		{
			get
			{
			  return fieldName;
			}
		}

		public virtual string ContentType
		{
			get
			{
			  return contentType;
			}
		}

		public virtual string TextContent
		{
			get
			{
			  return textContent;
			}
		}

		public virtual sbyte[] BinaryContent
		{
			get
			{
			  return binaryContent;
			}
		}

		public virtual string FileName
		{
			get
			{
			  return fileName;
			}
		}

	  }

	}

}