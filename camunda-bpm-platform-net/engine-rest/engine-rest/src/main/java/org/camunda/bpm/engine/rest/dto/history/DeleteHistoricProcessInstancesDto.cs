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
namespace org.camunda.bpm.engine.rest.dto.history
{

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteHistoricProcessInstancesDto
	{

	  protected internal IList<string> historicProcessInstanceIds;
	  protected internal HistoricProcessInstanceQueryDto historicProcessInstanceQuery;
	  protected internal string deleteReason;
	  protected internal bool failIfNotExists = true;

	  public virtual IList<string> HistoricProcessInstanceIds
	  {
		  get
		  {
			return historicProcessInstanceIds;
		  }
		  set
		  {
			this.historicProcessInstanceIds = value;
		  }
	  }


	  public virtual HistoricProcessInstanceQueryDto HistoricProcessInstanceQuery
	  {
		  get
		  {
			return historicProcessInstanceQuery;
		  }
		  set
		  {
			this.historicProcessInstanceQuery = value;
		  }
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