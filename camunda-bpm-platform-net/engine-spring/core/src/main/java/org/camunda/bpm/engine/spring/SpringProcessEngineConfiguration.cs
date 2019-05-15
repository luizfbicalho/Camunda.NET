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
namespace org.camunda.bpm.engine.spring
{
	using BeansException = org.springframework.beans.BeansException;
	using ApplicationContext = org.springframework.context.ApplicationContext;
	using ApplicationContextAware = org.springframework.context.ApplicationContextAware;


	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	public class SpringProcessEngineConfiguration : SpringTransactionsProcessEngineConfiguration, ApplicationContextAware
	{

	  protected internal ApplicationContext applicationContext;

	  protected internal override void initArtifactFactory()
	  {
		if (artifactFactory == null && applicationContext != null)
		{
		  artifactFactory = new SpringArtifactFactory(applicationContext);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setApplicationContext(org.springframework.context.ApplicationContext applicationContext) throws org.springframework.beans.BeansException
	  public override ApplicationContext ApplicationContext
	  {
		  set
		  {
			this.applicationContext = value;
		  }
	  }
	}

}