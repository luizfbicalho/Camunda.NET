using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.util.container
{

	using TestRule = org.junit.rules.TestRule;
	using Description = org.junit.runner.Description;
	using Statement = org.junit.runners.model.Statement;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TestContainerRule : TestRule
	{

	  private static readonly Logger LOGGER = Logger.getLogger(typeof(TestContainerRule).Name);

	  protected internal ContainerSpecifics containerSpecifics;

	  public virtual Statement apply(Statement @base, Description description)
	  {

		lookUpContainerSpecifics();
		TestRule containerSpecificRule = containerSpecifics.getTestRule(description.TestClass);
		return containerSpecificRule.apply(@base, description);
	  }

	  protected internal virtual void lookUpContainerSpecifics()
	  {

		if (this.containerSpecifics == null)
		{
		  ServiceLoader<ContainerSpecifics> serviceLoader = ServiceLoader.load(typeof(ContainerSpecifics));
		  IEnumerator<ContainerSpecifics> it = serviceLoader.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  if (it.hasNext())
		  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			ContainerSpecifics containerSpecifics = it.next();

			this.containerSpecifics = containerSpecifics;

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (it.hasNext())
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  LOGGER.warning("There is more than one test runtime container implementation present on the classpath. " + "Using " + containerSpecifics.GetType().FullName);
			}
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new Exception("Could not find container provider SPI that implements " + typeof(ContainerSpecifics).FullName);
		  }
		}

	  }
	}


}