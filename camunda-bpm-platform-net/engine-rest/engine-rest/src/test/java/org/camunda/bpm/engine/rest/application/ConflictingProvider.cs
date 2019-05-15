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
namespace org.camunda.bpm.engine.rest.application
{


	/// <summary>
	/// a provider that is in conflict with the jackson provider.
	/// 
	/// The produced content type has to be lexicographically sortable before 'application/json' to ensure it is picked instead of jackson
	/// by the JAX-RS runtime in case there is no content type defined in the response by the resource method.
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Provider @Produces("aaa/aaa") public class ConflictingProvider implements javax.ws.rs.ext.MessageBodyWriter<Object>
	public class ConflictingProvider : MessageBodyWriter<object>
	{
		public override bool isWriteable(Type type, Type genericType, Annotation[] annotations, MediaType mediaType)
		{
		return true;
		}

	  public override long getSize(object t, Type type, Type genericType, Annotation[] annotations, MediaType mediaType)
	  {
		return -1;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeTo(Object t, Class type, Type genericType, Annotation[] annotations, javax.ws.rs.core.MediaType mediaType, javax.ws.rs.core.MultivaluedMap<String, Object> httpHeaders, java.io.OutputStream entityStream) throws java.io.IOException, javax.ws.rs.WebApplicationException
	  public override void writeTo(object t, Type type, Type genericType, Annotation[] annotations, MediaType mediaType, MultivaluedMap<string, object> httpHeaders, Stream entityStream)
	  {
		entityStream.WriteByte("Conflicting provider used".GetBytes());
	  }

	}

}