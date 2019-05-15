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

	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbEntityLifecycleAware = org.camunda.bpm.engine.impl.db.DbEntityLifecycleAware;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using ByteArrayField = org.camunda.bpm.engine.impl.persistence.entity.util.ByteArrayField;
	using TypedValueField = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueField;
	using TypedValueUpdateListener = org.camunda.bpm.engine.impl.persistence.entity.util.TypedValueUpdateListener;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class VariableInstanceEntity : VariableInstance, CoreVariableInstance, ValueFields, DbEntity, DbEntityLifecycleAware, TypedValueUpdateListener, HasDbRevision, HasDbReferences
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			byteArrayField = new ByteArrayField(this, ResourceTypes.RUNTIME);
			typedValueField = new TypedValueField(this, true);
		}


	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal string name;

	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string taskId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string activityInstanceId;
	  protected internal string tenantId;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;

	  protected internal ByteArrayField byteArrayField;

	  protected internal TypedValueField typedValueField;

	  internal bool forcedUpdate;

	  protected internal string configuration;

	  protected internal long sequenceCounter = 1;

	  /// <summary>
	  /// <para>Determines whether this variable is supposed to be a local variable
	  /// in case of concurrency in its scope. This affects
	  /// </para>
	  /// 
	  /// <ul>
	  /// <li>tree expansion (not evaluated yet by the engine)
	  /// <li>activity instance IDs of variable instances: concurrentLocal
	  ///   variables always receive the activity instance id of their execution
	  ///   (which may not be the scope execution), while non-concurrentLocal variables
	  ///   always receive the activity instance id of their scope (which is set in the
	  ///   parent execution)
	  /// </ul>
	  /// 
	  /// <para>
	  ///   In the future, this field could be used for restoring the variable distribution
	  ///   when the tree is expanded/compacted multiple times.
	  ///   On expansion, the goal would be to keep concurrentLocal variables always with
	  ///   their concurrent replacing executions while non-concurrentLocal variables
	  ///   stay in the scope execution
	  /// </para>
	  /// </summary>
	  protected internal bool isConcurrentLocal = false;

	  /// <summary>
	  /// Determines whether this variable is stored in the data base.
	  /// </summary>
	  protected internal bool isTransient = false;

	  // transient properties
	  protected internal ExecutionEntity execution;

	  // Default constructor for SQL mapping
	  public VariableInstanceEntity()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		typedValueField.addImplicitUpdateListener(this);
	  }

	  public VariableInstanceEntity(string name, TypedValue value, bool isTransient) : this()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		this.name = name;
		this.isTransient = isTransient;
		typedValueField.setValue(value);
	  }

	  public static VariableInstanceEntity createAndInsert(string name, TypedValue value)
	  {
		VariableInstanceEntity variableInstance = create(name, value, value.Transient);
		insert(variableInstance);
		return variableInstance;
	  }

	  public static void insert(VariableInstanceEntity variableInstance)
	  {
		if (!variableInstance.Transient)
		{
		  Context.CommandContext.DbEntityManager.insert(variableInstance);
		}
	  }

	  public static VariableInstanceEntity create(string name, TypedValue value, bool isTransient)
	  {
		return new VariableInstanceEntity(name, value, isTransient);
	  }

	  public virtual void delete()
	  {

		if (!Transient)
		{
		  typedValueField.notifyImplicitValueUpdate();
		}

		// clear value
		clearValueFields();

		if (!isTransient)
		{
		  // delete variable
		  Context.CommandContext.DbEntityManager.delete(this);
		}
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			if (!string.ReferenceEquals(typedValueField.SerializerName, null))
			{
			  persistentState["serializerName"] = typedValueField.SerializerName;
			}
			if (longValue != null)
			{
			  persistentState["longValue"] = longValue;
			}
			if (doubleValue != null)
			{
			  persistentState["doubleValue"] = doubleValue;
			}
			if (!string.ReferenceEquals(textValue, null))
			{
			  persistentState["textValue"] = textValue;
			}
			if (!string.ReferenceEquals(textValue2, null))
			{
			  persistentState["textValue2"] = textValue2;
			}
			if (!string.ReferenceEquals(byteArrayField.ByteArrayId, null))
			{
			  persistentState["byteArrayValueId"] = byteArrayField.ByteArrayId;
			}
    
			persistentState["sequenceCounter"] = SequenceCounter;
			persistentState["concurrentLocal"] = isConcurrentLocal;
			persistentState["executionId"] = executionId;
			persistentState["taskId"] = taskId;
			persistentState["caseExecutionId"] = caseExecutionId;
			persistentState["caseInstanceId"] = caseInstanceId;
			persistentState["tenantId"] = tenantId;
			persistentState["processInstanceId"] = processInstanceId;
    
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  // lazy initialized relations ///////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  set
		  {
			this.caseInstanceId = value;
		  }
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  set
		  {
			this.caseExecutionId = value;
		  }
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual CaseExecutionEntity CaseExecution
	  {
		  set
		  {
			if (value != null)
			{
			  this.caseInstanceId = value.CaseInstanceId;
			  this.caseExecutionId = value.Id;
			  this.tenantId = value.TenantId;
			}
			else
			{
			  this.caseInstanceId = null;
			  this.caseExecutionId = null;
			  this.tenantId = null;
			}
		  }
		  get
		  {
			if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  return Context.CommandContext.CaseExecutionManager.findCaseExecutionById(caseExecutionId);
			}
			return null;
		  }
	  }

	  // byte array value /////////////////////////////////////////////////////////

	  // i couldn't find a easy readable way to extract the common byte array value logic
	  // into a common class.  therefor it's duplicated in VariableInstanceEntity,
	  // HistoricVariableInstance and HistoricDetailVariableInstanceUpdateEntity

	  public virtual string ByteArrayValueId
	  {
		  get
		  {
			return byteArrayField.ByteArrayId;
		  }
		  set
		  {
			this.byteArrayField.ByteArrayId = value;
		  }
	  }


	  public virtual sbyte[] ByteArrayValue
	  {
		  get
		  {
			return byteArrayField.getByteArrayValue();
		  }
		  set
		  {
			byteArrayField.setByteArrayValue(value, isTransient);
		  }
	  }


	  protected internal virtual void deleteByteArrayValue()
	  {
		byteArrayField.deleteByteArrayValue();
	  }

	  // type /////////////////////////////////////////////////////////////////////

	  public virtual object getValue()
	  {
		return typedValueField.getValue();
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return typedValueField.TypedValue;
		  }
	  }

	  public virtual TypedValue getTypedValue(bool deserializeValue)
	  {
		return typedValueField.getTypedValue(deserializeValue);
	  }

	  public virtual void setValue(TypedValue value)
	  {
		// clear value fields
		clearValueFields();

		typedValueField.setValue(value);
	  }

	  public virtual void clearValueFields()
	  {
		this.longValue = null;
		this.doubleValue = null;
		this.textValue = null;
		this.textValue2 = null;
		typedValueField.clear();

		if (!string.ReferenceEquals(byteArrayField.ByteArrayId, null))
		{
		  deleteByteArrayValue();
		  ByteArrayValueId = null;
		}
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return typedValueField.TypeName;
		  }
	  }

	  // entity lifecycle /////////////////////////////////////////////////////////

	  public virtual void postLoad()
	  {
		// make sure the serializer is initialized
		typedValueField.postLoad();
	  }

	  // execution ////////////////////////////////////////////////////////////////

	  protected internal virtual void ensureExecutionInitialized()
	  {
		if (execution == null && !string.ReferenceEquals(executionId, null))
		{
		  execution = Context.CommandContext.ExecutionManager.findExecutionById(executionId);
		}
	  }

	  public virtual ExecutionEntity Execution
	  {
		  get
		  {
			ensureExecutionInitialized();
			return execution;
		  }
		  set
		  {
			this.execution = value;
    
			if (value == null)
			{
			  this.executionId = null;
			  this.processInstanceId = null;
			  this.tenantId = null;
			}
			else
			{
			  ExecutionId = value.Id;
			  this.processInstanceId = value.ProcessInstanceId;
			  this.tenantId = value.TenantId;
			}
    
		  }
	  }


	  // case execution ///////////////////////////////////////////////////////////


	  // getters and setters //////////////////////////////////////////////////////

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


	  public virtual string TextValue
	  {
		  get
		  {
			return textValue;
		  }
		  set
		  {
			this.textValue = value;
		  }
	  }





	  public virtual long? LongValue
	  {
		  get
		  {
			return longValue;
		  }
		  set
		  {
			this.longValue = value;
		  }
	  }


	  public virtual double? DoubleValue
	  {
		  get
		  {
			return doubleValue;
		  }
		  set
		  {
			this.doubleValue = value;
		  }
	  }


	  public virtual string Name
	  {
		  set
		  {
			this.name = value;
		  }
		  get
		  {
			return name;
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


	  public virtual TypedValueSerializer<T1> Serializer<T1>
	  {
		  set
		  {
			typedValueField.SerializerName = value.Name;
		  }
		  get
		  {
			return typedValueField.Serializer;
		  }
	  }

	  public virtual string SerializerName
	  {
		  set
		  {
			typedValueField.SerializerName = value;
		  }
		  get
		  {
			return typedValueField.SerializerName;
		  }
	  }


	  public virtual string TextValue2
	  {
		  get
		  {
			return textValue2;
		  }
		  set
		  {
			this.textValue2 = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual TaskEntity Task
	  {
		  set
		  {
			if (value != null)
			{
			  this.taskId = value.Id;
			  this.tenantId = value.TenantId;
    
			  if (value.getExecution() != null)
			  {
				Execution = value.getExecution();
			  }
			  if (value.getCaseExecution() != null)
			  {
				CaseExecution = value.getCaseExecution();
			  }
			}
			else
			{
			  this.taskId = null;
			  this.tenantId = null;
			  Execution = null;
			  CaseExecution = null;
			}
    
    
		  }
		  get
		  {
			if (!string.ReferenceEquals(taskId, null))
			{
			  return Context.CommandContext.TaskManager.findTaskById(taskId);
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }



	  public virtual string ErrorMessage
	  {
		  get
		  {
			return typedValueField.ErrorMessage;
		  }
	  }

	  public virtual string VariableScopeId
	  {
		  get
		  {
			if (!string.ReferenceEquals(taskId, null))
			{
			  return taskId;
			}
    
			if (!string.ReferenceEquals(executionId, null))
			{
			  return executionId;
			}
    
			return caseExecutionId;
		  }
	  }

	  protected internal virtual VariableScope VariableScope
	  {
		  get
		  {
    
			if (!string.ReferenceEquals(taskId, null))
			{
			  return Task;
			}
			else if (!string.ReferenceEquals(executionId, null))
			{
			  return Execution;
			}
			else if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  return CaseExecution;
			}
			else
			{
			  return null;
			}
		  }
	  }


	  //sequence counter ///////////////////////////////////////////////////////////

	  public virtual long SequenceCounter
	  {
		  get
		  {
			return sequenceCounter;
		  }
		  set
		  {
			this.sequenceCounter = value;
		  }
	  }


	   public virtual void incrementSequenceCounter()
	   {
		sequenceCounter++;
	   }


	  public virtual bool ConcurrentLocal
	  {
		  get
		  {
			return isConcurrentLocal;
		  }
		  set
		  {
			this.isConcurrentLocal = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void onImplicitValueUpdate(final org.camunda.bpm.engine.variable.value.TypedValue updatedValue)
	  public virtual void onImplicitValueUpdate(TypedValue updatedValue)
	  {
		// note: this implementation relies on the
		//   behavior that the variable scope
		//   of variable value can never become null

		ProcessApplicationReference targetProcessApplication = ContextProcessApplication;
		if (targetProcessApplication != null)
		{
		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, updatedValue)
		 , targetProcessApplication, new InvocationContext(Execution));

		}
		else
		{
		  if (!isTransient)
		  {
			VariableScope.setVariableLocal(name, updatedValue);
		  }
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly VariableInstanceEntity outerInstance;

		  private TypedValue updatedValue;

		  public CallableAnonymousInnerClass(VariableInstanceEntity outerInstance, TypedValue updatedValue)
		  {
			  this.outerInstance = outerInstance;
			  this.updatedValue = updatedValue;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			outerInstance.VariableScope.setVariableLocal(outerInstance.name, updatedValue);
			return null;
		  }

	  }

	  protected internal virtual ProcessApplicationReference ContextProcessApplication
	  {
		  get
		  {
			if (!string.ReferenceEquals(taskId, null))
			{
			  return ProcessApplicationContextUtil.getTargetProcessApplication(Task);
			}
			else if (!string.ReferenceEquals(executionId, null))
			{
			  return ProcessApplicationContextUtil.getTargetProcessApplication(Execution);
			}
			else if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  return ProcessApplicationContextUtil.getTargetProcessApplication(CaseExecution);
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", name=" + name + ", processInstanceId=" + processInstanceId + ", executionId=" + executionId + ", caseInstanceId=" + caseInstanceId + ", caseExecutionId=" + caseExecutionId + ", taskId=" + taskId + ", activityInstanceId=" + activityInstanceId + ", tenantId=" + tenantId + ", longValue=" + longValue + ", doubleValue=" + doubleValue + ", textValue=" + textValue + ", textValue2=" + textValue2 + ", byteArrayValueId=" + ByteArrayValueId + ", configuration=" + configuration + ", isConcurrentLocal=" + isConcurrentLocal + "]";
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(id, null)) ? 0 : id.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		VariableInstanceEntity other = (VariableInstanceEntity) obj;
		if (string.ReferenceEquals(id, null))
		{
		  if (!string.ReferenceEquals(other.id, null))
		  {
			return false;
		  }
		}
		else if (!id.Equals(other.id))
		{
		  return false;
		}
		return true;
	  }

	  /// <param name="isTransient">
	  ///          <code>true</code>, if the variable is not stored in the data base.
	  ///          Default is <code>false</code>. </param>
	  public virtual bool Transient
	  {
		  set
		  {
			this.isTransient = value;
		  }
		  get
		  {
			return isTransient;
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
    
			if (!string.ReferenceEquals(processInstanceId, null))
			{
			  referenceIdAndClass[processInstanceId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(executionId, null))
			{
			  referenceIdAndClass[executionId] = typeof(ExecutionEntity);
			}
			if (!string.ReferenceEquals(caseInstanceId, null))
			{
			  referenceIdAndClass[caseInstanceId] = typeof(CaseExecutionEntity);
			}
			if (!string.ReferenceEquals(caseExecutionId, null))
			{
			  referenceIdAndClass[caseExecutionId] = typeof(CaseExecutionEntity);
			}
			if (!string.ReferenceEquals(ByteArrayValueId, null))
			{
			  referenceIdAndClass[ByteArrayValueId] = typeof(ByteArrayEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }
	}

}