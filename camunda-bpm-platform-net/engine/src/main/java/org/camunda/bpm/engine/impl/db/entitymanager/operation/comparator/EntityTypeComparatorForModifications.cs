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
namespace org.camunda.bpm.engine.impl.db.entitymanager.operation.comparator
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CaseSentryPartEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using org.camunda.bpm.engine.impl.persistence.entity;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;


	/// <summary>
	/// Compares operations by Entity type.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EntityTypeComparatorForModifications : IComparer<Type>
	{

	  public static readonly IDictionary<Type, int> TYPE_ORDER = new Dictionary<Type, int>();

	  static EntityTypeComparatorForModifications()
	  {

		// 1
		TYPE_ORDER[typeof(IncidentEntity)] = 1;
		TYPE_ORDER[typeof(VariableInstanceEntity)] = 1;
		TYPE_ORDER[typeof(IdentityLinkEntity)] = 1;

		TYPE_ORDER[typeof(EventSubscriptionEntity)] = 1;

		TYPE_ORDER[typeof(JobEntity)] = 1;
		TYPE_ORDER[typeof(MessageEntity)] = 1;
		TYPE_ORDER[typeof(TimerEntity)] = 1;
		TYPE_ORDER[typeof(EverLivingJobEntity)] = 1;

		TYPE_ORDER[typeof(MembershipEntity)] = 1;
		TYPE_ORDER[typeof(TenantMembershipEntity)] = 1;

		TYPE_ORDER[typeof(CaseSentryPartEntity)] = 1;

		TYPE_ORDER[typeof(ExternalTaskEntity)] = 1;
		TYPE_ORDER[typeof(Batch)] = 1;

		// 2
		TYPE_ORDER[typeof(TenantEntity)] = 2;
		TYPE_ORDER[typeof(GroupEntity)] = 2;
		TYPE_ORDER[typeof(UserEntity)] = 2;
		TYPE_ORDER[typeof(ByteArrayEntity)] = 2;
		TYPE_ORDER[typeof(TaskEntity)] = 2;
		TYPE_ORDER[typeof(JobDefinition)] = 2;

		// 3
		TYPE_ORDER[typeof(ExecutionEntity)] = 3;
		TYPE_ORDER[typeof(CaseExecutionEntity)] = 3;

		// 4
		TYPE_ORDER[typeof(ProcessDefinitionEntity)] = 4;
		TYPE_ORDER[typeof(CaseDefinitionEntity)] = 4;
		TYPE_ORDER[typeof(DecisionDefinitionEntity)] = 4;
		TYPE_ORDER[typeof(DecisionRequirementsDefinitionEntity)] = 4;
		TYPE_ORDER[typeof(ResourceEntity)] = 4;

		// 5
		TYPE_ORDER[typeof(DeploymentEntity)] = 5;

	  }

	  public virtual int Compare(Type firstEntityType, Type secondEntityType)
	  {

		if (firstEntityType == secondEntityType)
		{
		  return 0;
		}

		int? firstIndex = TYPE_ORDER[firstEntityType];
		int? secondIndex = TYPE_ORDER[secondEntityType];

		// unknown type happens before / after everything else
		if (firstIndex == null)
		{
		  firstIndex = int.MaxValue;
		}
		if (secondIndex == null)
		{
		  secondIndex = int.MaxValue;
		}

		int result = firstIndex.compareTo(secondIndex);
		if (result == 0)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  return firstEntityType.FullName.CompareTo(secondEntityType.FullName);

		}
		else
		{
		  return result;

		}
	  }

	}

}