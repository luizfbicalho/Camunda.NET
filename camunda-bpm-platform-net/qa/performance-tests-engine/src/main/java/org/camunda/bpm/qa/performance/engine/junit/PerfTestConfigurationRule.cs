﻿using System;
using System.IO;

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
namespace org.camunda.bpm.qa.performance.engine.junit
{

	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using PerfTestConfiguration = org.camunda.bpm.qa.performance.engine.framework.PerfTestConfiguration;
	using PerfTestException = org.camunda.bpm.qa.performance.engine.framework.PerfTestException;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// JUnit rule allowing to load the performance test configuration from a file
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestConfigurationRule : TestWatcher
	{

	  private const string PROPERTY_FILE_NAME = "perf-test-config.properties";

	  internal static PerfTestConfiguration perfTestConfiguration;

	  protected internal override void starting(Description description)
	  {
		if (perfTestConfiguration == null)
		{

		  File file = IoUtil.getFile(PROPERTY_FILE_NAME);
		  if (!file.exists())
		  {
			throw new PerfTestException("Cannot load file '" + PROPERTY_FILE_NAME + "': file does not exist.");
		  }
		  FileStream propertyInputStream = null;
		  try
		  {
			propertyInputStream = new FileStream(file, FileMode.Open, FileAccess.Read);
			Properties properties = new Properties();
			properties.load(propertyInputStream);
			perfTestConfiguration = new PerfTestConfiguration(properties);
		  }
		  catch (Exception e)
		  {
			throw new PerfTestException("Cannot load properties from file " + PROPERTY_FILE_NAME + ": " + e);

		  }
		  finally
		  {
			IoUtil.closeSilently(propertyInputStream);
		  }
		}
	  }

	  public virtual PerfTestConfiguration PerformanceTestConfiguration
	  {
		  get
		  {
			return perfTestConfiguration;
		  }
	  }

	}

}