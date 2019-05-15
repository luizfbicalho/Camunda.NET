﻿using System.Collections.Generic;

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
namespace org.camunda.connect.plugin.util
{

	using AbstractRequestInvocation = org.camunda.connect.impl.AbstractRequestInvocation;
	using ConnectorRequest = org.camunda.connect.spi.ConnectorRequest;
	using ConnectorRequestInterceptor = org.camunda.connect.spi.ConnectorRequestInterceptor;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TestRequestInvocation : AbstractRequestInvocation<object>
	{

	  public TestRequestInvocation<T1>(object target, ConnectorRequest<T1> request, IList<ConnectorRequestInterceptor> interceptorChain) : base(target, request, interceptorChain)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invokeTarget() throws Exception
	  public virtual object invokeTarget()
	  {
		return null;
	  }

	}

}