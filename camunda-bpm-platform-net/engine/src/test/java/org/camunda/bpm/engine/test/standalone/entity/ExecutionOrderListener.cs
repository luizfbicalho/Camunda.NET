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
namespace org.camunda.bpm.engine.test.standalone.entity
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ExecutionOrderListener : ExecutionListener
	{

	  protected internal static IList<ActivitySequenceCounterMap> activityExecutionOrder = new List<ActivitySequenceCounterMap>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		long sequenceCounter = executionEntity.SequenceCounter;
		string activityId = executionEntity.ActivityId;

		activityExecutionOrder.Add(new ActivitySequenceCounterMap(this, activityId, sequenceCounter));
	  }

	  public static void clearActivityExecutionOrder()
	  {
		activityExecutionOrder.Clear();
	  }

	  public static IList<ActivitySequenceCounterMap> ActivityExecutionOrder
	  {
		  get
		  {
			return activityExecutionOrder;
		  }
	  }

	  protected internal class ActivitySequenceCounterMap
	  {
		  private readonly ExecutionOrderListener outerInstance;


		protected internal string activityId;
		protected internal long sequenceCounter;

		public ActivitySequenceCounterMap(ExecutionOrderListener outerInstance, string activityId, long sequenceCounter)
		{
			this.outerInstance = outerInstance;
		  this.activityId = activityId;
		  this.sequenceCounter = sequenceCounter;
		}

		public virtual string ActivityId
		{
			get
			{
			  return activityId;
			}
		}

		public virtual long SequenceCounter
		{
			get
			{
			  return sequenceCounter;
			}
		}

	  }

	}

}