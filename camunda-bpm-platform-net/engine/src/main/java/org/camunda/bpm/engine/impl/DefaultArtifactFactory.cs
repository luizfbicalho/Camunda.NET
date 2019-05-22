using System;

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
namespace org.camunda.bpm.engine.impl
{

	/// <summary>
	/// Default ArtifactService implementation.
	/// This version uses Class.newInstance() to create
	/// new Artifacts.
	/// This is the default behaviour like has been in old
	/// camunda/activity versions.
	/// 
	/// @since 7.2.0
	/// @author <a href="mailto:struberg@yahoo.de">Mark Struberg</a>
	/// </summary>
	public class DefaultArtifactFactory : ArtifactFactory
	{
	  public virtual T getArtifact<T>(Type clazz)
	  {
			  clazz = typeof(T);
		try
		{
		  return System.Activator.CreateInstance(clazz);
		}
		catch (Exception e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("couldn't instantiate class " + clazz.FullName, e);
		}
	  }
	}

}