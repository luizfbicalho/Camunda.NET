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
namespace org.camunda.bpm.engine.impl.batch.removaltime
{


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SetRemovalTimeBatchConfiguration : BatchConfiguration
	{

	  protected internal DateTime removalTime;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool hasRemovalTime_Conflict;
	  protected internal bool isHierarchical;

	  public SetRemovalTimeBatchConfiguration(IList<string> ids) : base(ids)
	  {
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public virtual SetRemovalTimeBatchConfiguration setRemovalTime(DateTime removalTime)
	  {
		this.removalTime = removalTime;
		return this;
	  }

	  public virtual bool hasRemovalTime()
	  {
		return hasRemovalTime_Conflict;
	  }

	  public virtual SetRemovalTimeBatchConfiguration setHasRemovalTime(bool hasRemovalTime)
	  {
		this.hasRemovalTime_Conflict = hasRemovalTime;
		return this;
	  }

	  public virtual bool Hierarchical
	  {
		  get
		  {
			return isHierarchical;
		  }
	  }

	  public virtual SetRemovalTimeBatchConfiguration setHierarchical(bool hierarchical)
	  {
		isHierarchical = hierarchical;
		return this;
	  }

	}

}