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
namespace org.camunda.bpm.integrationtest.functional.el
{
	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationElResolver = org.camunda.bpm.application.ProcessApplicationElResolver;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class NullELResolver : ProcessApplicationElResolver
	{

	  /// <summary>
	  /// Spring and CDI have 100 and 200 respectively
	  /// </summary>
	  public virtual int? Precedence
	  {
		  get
		  {
			return 300;
		  }
	  }

	  public virtual ELResolver getElResolver(AbstractProcessApplication processApplication)
	  {
		return null;
	  }

	}

}