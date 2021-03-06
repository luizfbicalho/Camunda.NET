﻿using System;

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
namespace org.camunda.bpm.engine.exception.cmmn
{

	/// <summary>
	/// <para>This is exception is thrown when something happens in the execution
	/// of a case instance.</para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseException : ProcessEngineException
	{

	  private const long serialVersionUID = 1L;

	  public CaseException() : base()
	  {
	  }

	  public CaseException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public CaseException(string message) : base(message)
	  {
	  }

	  public CaseException(Exception cause) : base(cause)
	  {
	  }

	}

}