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
namespace org.camunda.bpm.engine.spring
{
	using DefaultArtifactFactory = org.camunda.bpm.engine.impl.DefaultArtifactFactory;
	using NoSuchBeanDefinitionException = org.springframework.beans.factory.NoSuchBeanDefinitionException;
	using ApplicationContext = org.springframework.context.ApplicationContext;

	/// 
	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	public class SpringArtifactFactory : ArtifactFactory
	{

	  private ArtifactFactory defaultArtifactFactory = new DefaultArtifactFactory();

	  private ApplicationContext applicationContext;

	  public SpringArtifactFactory(ApplicationContext applicationContext)
	  {
		this.applicationContext = applicationContext;
	  }

	  public virtual T getArtifact<T>(Type clazz)
	  {
			  clazz = typeof(T);
		T instance;

		try
		{
		  instance = applicationContext.getBean(clazz);
		}
		catch (NoSuchBeanDefinitionException)
		{
		  // fall back to using newInstance()
		  instance = defaultArtifactFactory.getArtifact(clazz);
		}

		return instance;
	  }
	}

}