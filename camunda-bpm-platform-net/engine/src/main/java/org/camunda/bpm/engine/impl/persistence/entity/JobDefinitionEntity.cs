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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class JobDefinitionEntity : JobDefinition, HasDbRevision, HasDbReferences, DbEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

	  /* Note: this is the id of the activity which is the cause that a Job is created.
	   * If the Job corresponds to an event scope, it may or may not correspond to the
	   * activity which defines the event scope.
	   *
	   * Example:
	   * user task with attached timer event:
	   * - timer event scope = user task
	   * - activity which causes the job to be created = timer event.
	   * => Job definition activityId will be activityId of the timer event, not the activityId of the user task.
	   */
	  protected internal string activityId;

	  /// <summary>
	  /// timer, message, ... </summary>
	  protected internal string jobType;
	  protected internal string jobConfiguration;

	  // job definition is active by default
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;

	  protected internal long? jobPriority;

	  protected internal string tenantId;

	  public JobDefinitionEntity()
	  {
	  }

	  public JobDefinitionEntity<T1>(JobDeclaration<T1> jobDeclaration)
	  {
		this.activityId = jobDeclaration.ActivityId;
		this.jobConfiguration = jobDeclaration.JobConfiguration;
		this.jobType = jobDeclaration.JobHandlerType;
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			Dictionary<string, object> state = new Dictionary<string, object>();
			state["processDefinitionId"] = processDefinitionId;
			state["processDefinitionKey"] = processDefinitionKey;
			state["activityId"] = activityId;
			state["jobType"] = jobType;
			state["jobConfiguration"] = jobConfiguration;
			state["suspensionState"] = suspensionState;
			state["jobPriority"] = jobPriority;
			state["tenantId"] = tenantId;
			return state;
		  }
	  }

	  // getters / setters /////////////////////////////////

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return SuspensionState_Fields.SUSPENDED.StateCode == suspensionState;
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


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual string JobType
	  {
		  get
		  {
			return jobType;
		  }
		  set
		  {
			this.jobType = value;
		  }
	  }


	  public virtual string JobConfiguration
	  {
		  get
		  {
			return jobConfiguration;
		  }
		  set
		  {
			this.jobConfiguration = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual int SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }


	  public virtual long? OverridingJobPriority
	  {
		  get
		  {
			return jobPriority;
		  }
	  }

	  public virtual long? JobPriority
	  {
		  set
		  {
			this.jobPriority = value;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
			return referenceIdAndClass;
		  }
	  }
	}

}