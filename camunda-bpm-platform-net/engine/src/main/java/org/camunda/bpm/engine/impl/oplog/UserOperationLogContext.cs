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
namespace org.camunda.bpm.engine.impl.oplog
{

	/// <summary>
	/// <para>Provides information about user operations.</para>
	/// 
	/// <para>One context object can contain many entries. An entry represents one operation on a set of
	/// resources of the same type. One such operation can change multiple properties on these entities.
	/// For example, more than one entry is needed when a cascading command is logged. Then there is an entry
	/// for the changes performed on the addressed resource type as well as entries for those resource types that
	/// are affected by the cascading behavior.</para>
	/// 
	/// @author Roman Smirnov
	/// @author Thorben Lindhauer
	/// </summary>
	public class UserOperationLogContext
	{

	  protected internal string operationId;
	  protected internal string userId;
	  protected internal IList<UserOperationLogContextEntry> entries;

	  public UserOperationLogContext()
	  {
		this.entries = new List<UserOperationLogContextEntry>();
	  }

	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			this.userId = value;
		  }
	  }


	  public virtual string OperationId
	  {
		  get
		  {
			return operationId;
		  }
		  set
		  {
			this.operationId = value;
		  }
	  }


	  public virtual void addEntry(UserOperationLogContextEntry entry)
	  {
		entries.Add(entry);
	  }

	  public virtual IList<UserOperationLogContextEntry> Entries
	  {
		  get
		  {
			return entries;
		  }
	  }



	}

}