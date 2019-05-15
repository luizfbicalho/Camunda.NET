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
namespace org.camunda.bpm.integrationtest.deployment.callbacks
{

	using Assert = org.junit.Assert;

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using PostDeployInjectApp = org.camunda.bpm.integrationtest.deployment.callbacks.apps.PostDeployInjectApp;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PostDeployInjectDefaultEngineTest
	public class PostDeployInjectDefaultEngineTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "test.war").addClass(typeof(PostDeployInjectApp));

		return archive;

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void test()
	  public virtual void test()
	  {
		Assert.assertNotNull("processEngine must be injected", PostDeployInjectApp.processEngine);
		Assert.assertNotNull("processApplicationInfo must be injected", PostDeployInjectApp.processApplicationInfo);

		IList<ProcessEngine> processEngines = PostDeployInjectApp.processEngines;
		Assert.assertNotNull("processEngines must be injected", processEngines);

		// the app did no do a deployment so no engines are in the list
		Assert.assertEquals(0, processEngines.Count);

	  }

	}

}