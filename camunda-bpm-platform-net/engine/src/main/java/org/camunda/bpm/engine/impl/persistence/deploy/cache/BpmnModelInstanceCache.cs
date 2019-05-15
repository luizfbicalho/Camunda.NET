using System;
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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class BpmnModelInstanceCache : ModelInstanceCache<BpmnModelInstance, ProcessDefinitionEntity>
	{

	  public BpmnModelInstanceCache(CacheFactory factory, int cacheCapacity, ResourceDefinitionCache<ProcessDefinitionEntity> definitionCache) : base(factory, cacheCapacity, definitionCache)
	  {
	  }

	  protected internal override void throwLoadModelException(string definitionId, Exception e)
	  {
		throw LOG.loadModelException("BPMN", "process", definitionId, e);
	  }

	  protected internal override BpmnModelInstance readModelFromStream(Stream bpmnResourceInputStream)
	  {
		return Bpmn.readModelFromStream(bpmnResourceInputStream);
	  }

	  protected internal override void logRemoveEntryFromDeploymentCacheFailure(string definitionId, Exception e)
	  {
		LOG.removeEntryFromDeploymentCacheFailure("process", definitionId, e);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> getAllDefinitionsForDeployment(final String deploymentId)
	  protected internal override IList<ProcessDefinition> getAllDefinitionsForDeployment(string deploymentId)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext = org.camunda.bpm.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;
		IList<ProcessDefinition> allDefinitionsForDeployment = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, deploymentId));
		return allDefinitionsForDeployment;
	  }

	  private class CallableAnonymousInnerClass : Callable<IList<ProcessDefinition>>
	  {
		  private readonly BpmnModelInstanceCache outerInstance;

		  private string deploymentId;

		  public CallableAnonymousInnerClass(BpmnModelInstanceCache outerInstance, string deploymentId)
		  {
			  this.outerInstance = outerInstance;
			  this.deploymentId = deploymentId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.repository.ProcessDefinition> call() throws Exception
		  public IList<ProcessDefinition> call()
		  {
			return (new ProcessDefinitionQueryImpl()).deploymentId(deploymentId).list();
		  }
	  }
	}

}