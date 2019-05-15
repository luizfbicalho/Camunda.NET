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
namespace org.camunda.bpm.engine.impl.cmmn.model
{

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CmmnOnPartDeclaration
	{

	  private const long serialVersionUID = 1L;

	  protected internal string standardEvent;
	  protected internal CmmnActivity source;
	  protected internal CmmnSentryDeclaration sentry;

	  public virtual string StandardEvent
	  {
		  get
		  {
			return standardEvent;
		  }
		  set
		  {
			this.standardEvent = value;
		  }
	  }


	  public virtual CmmnActivity Source
	  {
		  get
		  {
			return source;
		  }
		  set
		  {
			this.source = value;
		  }
	  }


	  public virtual CmmnSentryDeclaration Sentry
	  {
		  get
		  {
			return sentry;
		  }
		  set
		  {
			this.sentry = value;
		  }
	  }


	}

}