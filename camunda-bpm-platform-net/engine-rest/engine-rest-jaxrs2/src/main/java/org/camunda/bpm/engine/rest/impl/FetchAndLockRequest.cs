using System;

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

	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using FetchExternalTasksExtendedDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksExtendedDto;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class FetchAndLockRequest
	{

	  protected internal DateTime requestTime = ClockUtil.CurrentTime;
	  protected internal FetchExternalTasksExtendedDto dto;
	  protected internal AsyncResponse asyncResponse;
	  protected internal string processEngineName;
	  protected internal Authentication authentication;

	  public virtual DateTime RequestTime
	  {
		  get
		  {
			return requestTime;
		  }
	  }

	  public virtual FetchAndLockRequest setRequestTime(DateTime requestTime)
	  {
		this.requestTime = requestTime;
		return this;
	  }

	  public virtual FetchExternalTasksExtendedDto Dto
	  {
		  get
		  {
			return dto;
		  }
	  }

	  public virtual FetchAndLockRequest setDto(FetchExternalTasksExtendedDto dto)
	  {
		this.dto = dto;
		return this;
	  }

	  public virtual AsyncResponse AsyncResponse
	  {
		  get
		  {
			return asyncResponse;
		  }
	  }

	  public virtual FetchAndLockRequest setAsyncResponse(AsyncResponse asyncResponse)
	  {
		this.asyncResponse = asyncResponse;
		return this;
	  }

	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
	  }

	  public virtual FetchAndLockRequest setProcessEngineName(string processEngineName)
	  {
		this.processEngineName = processEngineName;
		return this;
	  }

	  public virtual Authentication Authentication
	  {
		  get
		  {
			return authentication;
		  }
	  }

	  public virtual FetchAndLockRequest setAuthentication(Authentication authentication)
	  {
		this.authentication = authentication;
		return this;
	  }

	  public virtual long TimeoutTimestamp
	  {
		  get
		  {
			FetchExternalTasksExtendedDto dto = Dto;
			long requestTime = RequestTime.Ticks;
			long asyncResponseTimeout = dto.AsyncResponseTimeout.Value;
			return requestTime + asyncResponseTimeout;
		  }
	  }

	  public override string ToString()
	  {
		return "FetchAndLockRequest [requestTime=" + requestTime + ", dto=" + dto + ", asyncResponse=" + asyncResponse + ", processEngineName=" + processEngineName + ", authentication=" + authentication + "]";
	  }

	}

}