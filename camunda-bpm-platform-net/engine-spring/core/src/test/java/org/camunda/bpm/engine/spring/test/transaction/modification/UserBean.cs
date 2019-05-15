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
namespace org.camunda.bpm.engine.spring.test.transaction.modification
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using Component = org.springframework.stereotype.Component;
	using Transactional = org.springframework.transaction.annotation.Transactional;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Component public class UserBean
	public class UserBean
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired public org.camunda.bpm.engine.ProcessEngine processEngine;
		public ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired RuntimeService runtimeService;
	  internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired RepositoryService repositoryService;
	  internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transactional public void completeUserTaskAndModifyInstanceInOneTransaction(org.camunda.bpm.engine.runtime.ProcessInstance procInst)
	  public virtual void completeUserTaskAndModifyInstanceInOneTransaction(ProcessInstance procInst)
	  {
		// this method assures that the execution the process instance
		// modification is done in one transaction.

		// reset the process instance before the timer
		runtimeService.createProcessInstanceModification(procInst.Id).cancelAllForActivity("TimerEvent").startBeforeActivity("TimerEvent").execute();
	  }

	}


}