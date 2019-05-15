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
namespace org.camunda.bpm.engine.impl.batch
{


	public class BatchConfiguration
	{

	  protected internal IList<string> ids;
	  protected internal bool failIfNotExists;

	  public BatchConfiguration(IList<string> ids) : this(ids, true)
	  {
	  }

	  public BatchConfiguration(IList<string> ids, bool failIfNotExists)
	  {
		this.ids = ids;
		this.failIfNotExists = failIfNotExists;
	  }

	  public virtual IList<string> Ids
	  {
		  get
		  {
			return ids;
		  }
		  set
		  {
			this.ids = value;
		  }
	  }


	  public virtual bool FailIfNotExists
	  {
		  get
		  {
			return failIfNotExists;
		  }
		  set
		  {
			this.failIfNotExists = value;
		  }
	  }


	}

}