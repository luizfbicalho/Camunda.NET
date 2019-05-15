﻿using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.container.impl.threading.ra.outbound
{

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JcaExecutorServiceConnectionImpl : JcaExecutorServiceConnection
	{

	  protected internal JcaExecutorServiceManagedConnection mc;
	  protected internal JcaExecutorServiceManagedConnectionFactory mcf;

	  public JcaExecutorServiceConnectionImpl()
	  {
	  }

	  public JcaExecutorServiceConnectionImpl(JcaExecutorServiceManagedConnection mc, JcaExecutorServiceManagedConnectionFactory mcf)
	  {
		this.mc = mc;
		this.mcf = mcf;
	  }

	  public virtual void closeConnection()
	  {
		mc.closeHandle(this);
	  }

	  public virtual bool schedule(ThreadStart runnable, bool isLongRunning)
	  {
		return mc.schedule(runnable, isLongRunning);
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return mc.getExecuteJobsRunnable(jobIds, processEngine);
	  }



	}

}