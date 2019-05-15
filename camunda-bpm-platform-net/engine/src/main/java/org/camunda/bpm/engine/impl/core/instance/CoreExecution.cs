using System;

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
namespace org.camunda.bpm.engine.impl.core.instance
{
	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;

	/// <summary>
	/// Defines the base API for the execution of an activity.
	/// 
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// 
	/// </summary>
	[Serializable]
	public abstract class CoreExecution : AbstractVariableScope, BaseDelegateExecution
	{
		public abstract string BusinessKey {get;}

	  private const long serialVersionUID = 1L;

	  private static readonly CoreLogger LOG = CoreLogger.CORE_LOGGER;

	  protected internal string id;

	  /// <summary>
	  /// the business key for this execution
	  /// </summary>
	  protected internal string businessKey;
	  protected internal string businessKeyWithoutCascade;

	  protected internal string tenantId;

	  // events ///////////////////////////////////////////////////////////////////

	  protected internal string eventName;
	  protected internal CoreModelElement eventSource;
	  protected internal int listenerIndex = 0;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMapping;
	  protected internal bool skipSubprocesses;

	  // atomic operations ////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends CoreExecution> void performOperation(org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation<T> operation)
	  public virtual void performOperation<T>(CoreAtomicOperation<T> operation) where T : CoreExecution
	  {
		LOG.debugPerformingAtomicOperation(operation, this);
		operation.execute((T) this);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends CoreExecution> void performOperationSync(org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation<T> operation)
	  public virtual void performOperationSync<T>(CoreAtomicOperation<T> operation) where T : CoreExecution
	  {
		LOG.debugPerformingAtomicOperation(operation, this);
		operation.execute((T) this);
	  }

	  // event handling ////////////////////////////////////////////////////////

	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }


	  public virtual CoreModelElement EventSource
	  {
		  get
		  {
			return eventSource;
		  }
		  set
		  {
			this.eventSource = value;
		  }
	  }


	  public virtual int ListenerIndex
	  {
		  get
		  {
			return listenerIndex;
		  }
		  set
		  {
			this.listenerIndex = value;
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void invokeListener(org.camunda.bpm.engine.delegate.DelegateListener listener) throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void invokeListener(DelegateListener listener)
	  {
		listener.notify(this);
	  }

	  // getters / setters /////////////////////////////////////////////////

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


	  public virtual string BusinessKeyWithoutCascade
	  {
		  get
		  {
			return businessKeyWithoutCascade;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  set
		  {
			this.businessKey = value;
			this.businessKeyWithoutCascade = value;
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
			return skipIoMapping;
		  }
		  set
		  {
			this.skipIoMapping = value;
		  }
	  }


	  public virtual bool SkipSubprocesses
	  {
		  get
		  {
			return skipSubprocesses;
		  }
	  }

	  public virtual bool SkipSubprocesseses
	  {
		  set
		  {
			this.skipSubprocesses = value;
		  }
	  }

	}

}