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
namespace org.camunda.bpm.engine.exception.dmn
{
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;


	/// <summary>
	/// <para>This exception is thrown when a <seealso cref="DmnModelInstance"/> is not found.</para>
	/// </summary>
	public class DmnModelInstanceNotFoundException : DecisionException
	{

	  private const long serialVersionUID = 1L;

	  public DmnModelInstanceNotFoundException() : base()
	  {
	  }

	  public DmnModelInstanceNotFoundException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public DmnModelInstanceNotFoundException(string message) : base(message)
	  {
	  }

	  public DmnModelInstanceNotFoundException(Exception cause) : base(cause)
	  {
	  }

	}

}