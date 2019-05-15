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
namespace org.camunda.bpm.engine
{
	/// <summary>
	/// <para>Exception resulting from a bad user request. A bad user request is
	/// an interaction where the user requests some non-existing state or
	/// attempts to perform an illegal action on some entity.</para>
	/// 
	/// <para><strong>Examples:</strong>
	/// <ul>
	///  <li>cancelling a non-existing process instance</li>
	///  <li>triggering a suspended execution...</li>
	/// </ul>
	/// </para>
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class BadUserRequestException : ProcessEngineException
	{

	  private const long serialVersionUID = 1L;

	  public BadUserRequestException()
	  {
	  }

	  public BadUserRequestException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public BadUserRequestException(string message) : base(message)
	  {
	  }

	  public BadUserRequestException(Exception cause) : base(cause)
	  {
	  }

	}

}