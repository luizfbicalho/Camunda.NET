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
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// @author Askar Akhmerov
	/// </summary>
	public class HandleExternalTaskFailureCmd : HandleExternalTaskCmd
	{

	  protected internal string errorMessage;
	  protected internal string errorDetails;
	  protected internal long retryDuration;
	  protected internal int retries;

	  public HandleExternalTaskFailureCmd(string externalTaskId, string workerId, string errorMessage, int retries, long retryDuration) : this(externalTaskId,workerId,errorMessage,null,retries,retryDuration)
	  {
	  }

	  /// <summary>
	  /// Overloaded constructor to support short and full error messages
	  /// </summary>
	  /// <param name="externalTaskId"> </param>
	  /// <param name="workerId"> </param>
	  /// <param name="errorMessage"> </param>
	  /// <param name="errorDetails"> </param>
	  /// <param name="retries"> </param>
	  /// <param name="retryDuration"> </param>
	  public HandleExternalTaskFailureCmd(string externalTaskId, string workerId, string errorMessage, string errorDetails, int retries, long retryDuration) : base(externalTaskId, workerId)
	  {
		this.errorMessage = errorMessage;
		this.errorDetails = errorDetails;
		this.retries = retries;
		this.retryDuration = retryDuration;
	  }

	  public virtual void execute(ExternalTaskEntity externalTask)
	  {
		externalTask.failed(errorMessage, errorDetails, retries, retryDuration);
	  }

	  protected internal override void validateInput()
	  {
		base.validateInput();
		EnsureUtil.ensureGreaterThanOrEqual("retries", retries, 0);
		EnsureUtil.ensureGreaterThanOrEqual("retryDuration", retryDuration, 0);
	  }

	  public override string ErrorMessageOnWrongWorkerAccess
	  {
		  get
		  {
			return "Failure of External Task " + externalTaskId + " cannot be reported by worker '" + workerId;
		  }
	  }
	}

}