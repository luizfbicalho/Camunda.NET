using System;
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
	using FileItemIterator = org.apache.commons.fileupload.FileItemIterator;
	using FileItemStream = org.apache.commons.fileupload.FileItemStream;
	using FileUpload = org.apache.commons.fileupload.FileUpload;
	using RequestContext = org.apache.commons.fileupload.RequestContext;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using FormPart = org.camunda.bpm.engine.rest.mapper.MultipartFormData.FormPart;


	/// <summary>
	/// <para>Provides a <seealso cref="MessageBodyReader"/> for <seealso cref="MultipartFormData"/>. This allows writing resources which
	/// consume <seealso cref="MediaType.MULTIPART_FORM_DATA"/> which is parsed into a <seealso cref="MultipartFormData"/> object:</para>
	/// 
	/// <pre>
	/// {@literal @}POST
	/// {@literal @}Consumes(MediaType.MULTIPART_FORM_DATA)
	/// void handleMultipartPost(MultipartFormData multipartFormData);
	/// </pre>
	/// 
	/// <para>The implementation used apache commons fileupload in order to parse the request and populate an instance of
	/// <seealso cref="MultipartFormData"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Provider @Consumes(MediaType.MULTIPART_FORM_DATA) public class MultipartPayloadProvider implements javax.ws.rs.ext.MessageBodyReader<MultipartFormData>
	public class MultipartPayloadProvider : MessageBodyReader<MultipartFormData>
	{

	  public const string TYPE_NAME = "multipart";
	  public const string SUB_TYPE_NAME = "form-data";

	  public virtual bool isReadable(Type type, Type genericType, Annotation[] annotations, MediaType mediaType)
	  {
		return TYPE_NAME.Equals(mediaType.Type.ToLower()) && SUB_TYPE_NAME.Equals(mediaType.Subtype.ToLower());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MultipartFormData readFrom(Class<MultipartFormData> type, Type genericType, Annotation[] annotations, javax.ws.rs.core.MediaType mediaType, javax.ws.rs.core.MultivaluedMap<String, String> httpHeaders, java.io.InputStream entityStream) throws java.io.IOException, javax.ws.rs.WebApplicationException
	  public virtual MultipartFormData readFrom(Type type, Type genericType, Annotation[] annotations, MediaType mediaType, MultivaluedMap<string, string> httpHeaders, Stream entityStream)
	  {
			  type = typeof(MultipartFormData);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MultipartFormData multipartFormData = createMultipartFormDataInstance();
		MultipartFormData multipartFormData = createMultipartFormDataInstance();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.fileupload.FileUpload fileUpload = createFileUploadInstance();
		FileUpload fileUpload = createFileUploadInstance();

		string contentType = httpHeaders.getFirst("content-type");
		RestMultipartRequestContext requestContext = createRequestContext(entityStream, contentType);

		// parse the request (populates the multipartFormData)
		parseRequest(multipartFormData, fileUpload, requestContext);

		return multipartFormData;

	  }

	  protected internal virtual FileUpload createFileUploadInstance()
	  {
		return new FileUpload();
	  }

	  protected internal virtual MultipartFormData createMultipartFormDataInstance()
	  {
		return new MultipartFormData();
	  }

	  protected internal virtual void parseRequest(MultipartFormData multipartFormData, FileUpload fileUpload, RestMultipartRequestContext requestContext)
	  {
		try
		{
		  FileItemIterator itemIterator = fileUpload.getItemIterator(requestContext);
		  while (itemIterator.hasNext())
		  {
			FileItemStream stream = itemIterator.next();
			multipartFormData.addPart(new FormPart(stream));
		  }
		}
		catch (Exception e)
		{
		  throw new RestException(Status.BAD_REQUEST, e, "multipart/form-data cannot be processed");

		}
	  }

	  protected internal virtual RestMultipartRequestContext createRequestContext(Stream entityStream, string contentType)
	  {
		return new RestMultipartRequestContext(entityStream, contentType);
	  }

	  /// <summary>
	  /// Exposes the REST request to commons fileupload
	  /// 
	  /// </summary>
	  internal class RestMultipartRequestContext : RequestContext
	  {

		protected internal Stream inputStream;
		protected internal string contentType;

		public RestMultipartRequestContext(Stream inputStream, string contentType)
		{
		  this.inputStream = inputStream;
		  this.contentType = contentType;
		}

		public virtual string CharacterEncoding
		{
			get
			{
			  return null;
			}
		}

		public virtual string ContentType
		{
			get
			{
			  return contentType;
			}
		}

		public virtual int ContentLength
		{
			get
			{
			  return -1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStream() throws java.io.IOException
		public virtual Stream InputStream
		{
			get
			{
			  return inputStream;
			}
		}

	  }

	}

}