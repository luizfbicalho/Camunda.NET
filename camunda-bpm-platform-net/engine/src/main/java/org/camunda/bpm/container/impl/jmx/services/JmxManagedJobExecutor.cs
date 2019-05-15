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
namespace org.camunda.bpm.container.impl.jmx.services
{
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JmxManagedJobExecutor : PlatformService<JobExecutor>, JmxManagedJobExecutorMBean
	{

	  protected internal readonly JobExecutor jobExecutor;

	  public JmxManagedJobExecutor(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
	  }

	  public virtual void start(PlatformServiceContainer mBeanServiceContainer)
	  {
		// no-op:
		// job executor is lazy-started when first process engine is registered and jobExecutorActivate = true
		// See: #CAM-4817
	  }

	  public virtual void stop(PlatformServiceContainer mBeanServiceContainer)
	  {
		shutdown();
	  }

	  public virtual void start()
	  {
		jobExecutor.start();
	  }

	  public virtual void shutdown()
	  {
		jobExecutor.shutdown();
	  }

	  public virtual int WaitTimeInMillis
	  {
		  get
		  {
			return jobExecutor.WaitTimeInMillis;
		  }
		  set
		  {
			jobExecutor.WaitTimeInMillis = value;
		  }
	  }


	  public virtual int LockTimeInMillis
	  {
		  get
		  {
			return jobExecutor.LockTimeInMillis;
		  }
		  set
		  {
			jobExecutor.LockTimeInMillis = value;
		  }
	  }


	  public virtual string LockOwner
	  {
		  get
		  {
			return jobExecutor.LockOwner;
		  }
		  set
		  {
			jobExecutor.LockOwner = value;
		  }
	  }


	  public virtual int MaxJobsPerAcquisition
	  {
		  get
		  {
			return jobExecutor.MaxJobsPerAcquisition;
		  }
		  set
		  {
			jobExecutor.MaxJobsPerAcquisition = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return jobExecutor.Name;
		  }
	  }

	  public virtual JobExecutor Value
	  {
		  get
		  {
			return jobExecutor;
		  }
	  }

	  public virtual bool Active
	  {
		  get
		  {
			return jobExecutor.Active;
		  }
	  }
	}

}