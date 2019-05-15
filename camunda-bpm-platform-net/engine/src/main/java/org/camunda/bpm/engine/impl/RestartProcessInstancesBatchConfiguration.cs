﻿using System.Collections.Generic;

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

	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;

	/// 
	/// <summary>
	/// @author Anna Pazola
	/// 
	/// </summary>
	public class RestartProcessInstancesBatchConfiguration : BatchConfiguration
	{

	  protected internal IList<AbstractProcessInstanceModificationCommand> instructions;
	  protected internal string processDefinitionId;
	  protected internal bool initialVariables;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;
	  protected internal bool withoutBusinessKey;

	  public RestartProcessInstancesBatchConfiguration(IList<string> processInstanceIds, IList<AbstractProcessInstanceModificationCommand> instructions, string processDefinitionId, bool initialVariables, bool skipCustomListeners, bool skipIoMappings, bool withoutBusinessKey) : base(processInstanceIds)
	  {
		this.instructions = instructions;
		this.processDefinitionId = processDefinitionId;
		this.initialVariables = initialVariables;
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;
		this.withoutBusinessKey = withoutBusinessKey;
	  }

	  public virtual IList<AbstractProcessInstanceModificationCommand> Instructions
	  {
		  get
		  {
			return instructions;
		  }
		  set
		  {
			this.instructions = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual bool InitialVariables
	  {
		  get
		  {
			return initialVariables;
		  }
		  set
		  {
			this.initialVariables = value;
		  }
	  }


	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners;
		  }
		  set
		  {
			this.skipCustomListeners = value;
		  }
	  }


	  public virtual bool SkipIoMappings
	  {
		  get
		  {
			return skipIoMappings;
		  }
		  set
		  {
			this.skipIoMappings = value;
		  }
	  }


	  public virtual bool WithoutBusinessKey
	  {
		  get
		  {
			return withoutBusinessKey;
		  }
		  set
		  {
			this.withoutBusinessKey = value;
		  }
	  }

	}

}