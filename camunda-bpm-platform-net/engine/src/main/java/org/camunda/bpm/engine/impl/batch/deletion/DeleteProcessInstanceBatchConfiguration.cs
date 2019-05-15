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
namespace org.camunda.bpm.engine.impl.batch.deletion
{

	/// <summary>
	/// Configuration object that is passed to the Job that will actually perform execution of
	/// deletion.
	/// <para>
	/// This object will be serialized and persisted as run will be performed asynchronously.
	/// 
	/// @author Askar Akhmerov
	/// </para>
	/// </summary>
	/// <seealso cref= org.camunda.bpm.engine.impl.batch.deletion.DeleteProcessInstanceBatchConfigurationJsonConverter </seealso>
	public class DeleteProcessInstanceBatchConfiguration : BatchConfiguration
	{
	  protected internal string deleteReason;
	  protected internal bool skipCustomListeners;
	  protected internal bool skipSubprocesses;

	  public DeleteProcessInstanceBatchConfiguration(IList<string> ids, bool skipCustomListeners, bool skipSubprocesses) : this(ids, null, skipCustomListeners, skipSubprocesses, true)
	  {
	  }

	  public DeleteProcessInstanceBatchConfiguration(IList<string> ids, string deleteReason, bool skipCustomListeners) : this(ids, deleteReason, skipCustomListeners, true, true)
	  {
	  }

	  public DeleteProcessInstanceBatchConfiguration(IList<string> ids, string deleteReason, bool skipCustomListeners, bool skipSubprocesses, bool failIfNotExists) : base(ids)
	  {
		this.deleteReason = deleteReason;
		this.skipCustomListeners = skipCustomListeners;
		this.skipSubprocesses = skipSubprocesses;
		this.failIfNotExists = failIfNotExists;
	  }

	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
		  set
		  {
			this.deleteReason = value;
		  }
	  }


	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners;
		  }
	  }

	  public virtual bool SkipSubprocesses
	  {
		  get
		  {
			return skipSubprocesses;
		  }
		  set
		  {
			this.skipSubprocesses = value;
		  }
	  }


	}

}