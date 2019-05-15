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
namespace org.camunda.bpm.engine.impl.cmd
{
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// 
	/// <summary>
	/// @author Anna.Pazola
	/// 
	/// </summary>
	public class ExtendLockOnExternalTaskCmd : HandleExternalTaskCmd
	{

	  private long newLockTime;

	  public ExtendLockOnExternalTaskCmd(string externalTaskId, string workerId, long newLockTime) : base(externalTaskId, workerId)
	  {
		EnsureUtil.ensurePositive(typeof(BadUserRequestException), "lockTime", newLockTime);
		this.newLockTime = newLockTime;
	  }

	  public override string ErrorMessageOnWrongWorkerAccess
	  {
		  get
		  {
			return "The lock of the External Task " + externalTaskId + " cannot be extended by worker '" + workerId + "'";
		  }
	  }

	  protected internal virtual void execute(ExternalTaskEntity externalTask)
	  {
		EnsureUtil.ensureGreaterThanOrEqual(typeof(BadUserRequestException), "Cannot extend a lock that expired", "lockExpirationTime", externalTask.LockExpirationTime.Ticks, ClockUtil.CurrentTime.Ticks);
		externalTask.extendLock(newLockTime);
	  }
	}

}