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
namespace org.camunda.bpm.integrationtest.deployment.callbacks.apps
{

	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ProcessApplication public class PostDeployInjectApp extends org.camunda.bpm.application.impl.ServletProcessApplication
	public class PostDeployInjectApp : ServletProcessApplication
	{

	  public static ProcessEngine processEngine;
	  public static ProcessApplicationInfo processApplicationInfo;
	  public static IList<ProcessEngine> processEngines;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostDeploy public void injectDefaultEngine(org.camunda.bpm.engine.ProcessEngine processEngine, java.util.List<org.camunda.bpm.engine.ProcessEngine> processEngines, org.camunda.bpm.application.ProcessApplicationInfo processApplicationInfo)
	  public virtual void injectDefaultEngine(ProcessEngine processEngine, IList<ProcessEngine> processEngines, ProcessApplicationInfo processApplicationInfo)
	  {
		PostDeployInjectApp.processEngine = processEngine;
		PostDeployInjectApp.processEngines = processEngines;
		PostDeployInjectApp.processApplicationInfo = processApplicationInfo;
	  }

	}

}