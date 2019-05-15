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
namespace org.camunda.bpm.engine.impl.management
{
	using CachePurgeReport = org.camunda.bpm.engine.impl.persistence.deploy.cache.CachePurgeReport;

	/// <summary>
	/// The purge report contains information about the deleted rows for each table
	/// and also the deleted values which are removed from the deployment cache.
	/// If now entities are deleted since the database was already clean the purge report is empty.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class PurgeReport
	{

	  private DatabasePurgeReport databasePurgeReport;
	  private CachePurgeReport cachePurgeReport;

	  public virtual DatabasePurgeReport DatabasePurgeReport
	  {
		  get
		  {
			return databasePurgeReport;
		  }
		  set
		  {
			this.databasePurgeReport = value;
		  }
	  }


	  public virtual CachePurgeReport CachePurgeReport
	  {
		  get
		  {
			return cachePurgeReport;
		  }
		  set
		  {
			this.cachePurgeReport = value;
		  }
	  }


	  public virtual bool Empty
	  {
		  get
		  {
			return cachePurgeReport.Empty && databasePurgeReport.Empty;
		  }
	  }
	}

}