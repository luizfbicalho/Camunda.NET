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
namespace org.camunda.bpm.engine.impl.util.io
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ResourceStreamSource : StreamSource
	{

	  internal string resource;
	  internal ClassLoader classLoader;

	  public ResourceStreamSource(string resource)
	  {
		this.resource = resource;
	  }

	  public ResourceStreamSource(string resource, ClassLoader classLoader)
	  {
		this.resource = resource;
		this.classLoader = classLoader;
	  }

	  public virtual Stream InputStream
	  {
		  get
		  {
			Stream inputStream = null;
			if (classLoader == null)
			{
			  inputStream = ReflectUtil.getResourceAsStream(resource);
			}
			else
			{
			  classLoader.getResourceAsStream(resource);
			}
			ensureNotNull("resource '" + resource + "' doesn't exist", "inputStream", inputStream);
			return inputStream;
		  }
	  }

	  public override string ToString()
	  {
		return "Resource[" + resource + "]";
	  }
	}

}