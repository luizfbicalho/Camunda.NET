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
namespace org.camunda.bpm.engine.rest.impl
{

	using LockedExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.LockedExternalTaskDto;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class FetchAndLockResult
	{

	  protected internal IList<LockedExternalTaskDto> tasks = new List<LockedExternalTaskDto>();
	  protected internal Exception throwable;

	  public FetchAndLockResult(IList<LockedExternalTaskDto> tasks)
	  {
		this.tasks = tasks;
	  }

	  public FetchAndLockResult(Exception throwable)
	  {
		this.throwable = throwable;
	  }

	  public virtual IList<LockedExternalTaskDto> Tasks
	  {
		  get
		  {
			return tasks;
		  }
	  }

	  public virtual Exception Throwable
	  {
		  get
		  {
			return throwable;
		  }
	  }

	  public virtual bool wasSuccessful()
	  {
		return throwable == null;
	  }

	  public static FetchAndLockResult successful(IList<LockedExternalTaskDto> tasks)
	  {
		return new FetchAndLockResult(tasks);
	  }

	  public static FetchAndLockResult failed(Exception throwable)
	  {
		return new FetchAndLockResult(throwable);
	  }

	  public override string ToString()
	  {
		return "FetchAndLockResult [tasks=" + tasks + ", throwable=" + throwable + "]";
	  }

	}
}