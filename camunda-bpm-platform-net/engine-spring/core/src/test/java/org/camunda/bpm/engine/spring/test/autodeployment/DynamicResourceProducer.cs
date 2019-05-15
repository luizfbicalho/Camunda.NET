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
namespace org.camunda.bpm.engine.spring.test.autodeployment
{

	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ByteArrayResource = org.springframework.core.io.ByteArrayResource;
	using Resource = org.springframework.core.io.Resource;

	public class DynamicResourceProducer
	{

	  private static IList<Resource> resources = new List<Resource>();

	  public static void clearResources()
	  {
		resources.Clear();
	  }

	  public static void addResource(string name, BpmnModelInstance modelInstance)
	  {
		MemoryStream outStream = new MemoryStream();
		Bpmn.writeModelToStream(outStream, modelInstance);

		resources.Add(new NamedByteArrayResource(outStream.toByteArray(), name));
	  }

	  public static Resource[] Resources
	  {
		  get
		  {
			return resources.ToArray();
		  }
	  }

	  /*
	   * In Spring 5, #getDescription is implemented differently
	   */
	  public class NamedByteArrayResource : ByteArrayResource
	  {
		internal string description;
		public NamedByteArrayResource(sbyte[] byteArray, string description) : base(byteArray, description)
		{
		  this.description = description;
		}

		public override string Description
		{
			get
			{
			  return description;
			}
		}
	  }
	}

}