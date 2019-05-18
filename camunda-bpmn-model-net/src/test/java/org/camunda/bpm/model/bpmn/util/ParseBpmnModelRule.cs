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
namespace org.camunda.bpm.model.bpmn.util
{
	using IoUtil = org.camunda.bpm.model.xml.impl.util.IoUtil;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ParseBpmnModelRule : TestWatcher
	{

	  protected internal BpmnModelInstance bpmnModelInstance;

	  protected internal override void starting(Description description)
	  {

		if (description.getAnnotation(typeof(BpmnModelResource)) != null)
		{

		  Type testClass = description.TestClass;
		  string methodName = description.MethodName;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  string resourceFolderName = testClass.FullName.replaceAll("\\.", "/");
		  string bpmnResourceName = resourceFolderName + "." + methodName + ".bpmn";

		  Stream resourceAsStream = this.GetType().ClassLoader.getResourceAsStream(bpmnResourceName);
		  try
		  {
			bpmnModelInstance = Bpmn.readModelFromStream(resourceAsStream);
		  }
		  finally
		  {
			IoUtil.closeSilently(resourceAsStream);
		  }

		}

	  }

	  public virtual BpmnModelInstance BpmnModel
	  {
		  get
		  {
			return bpmnModelInstance;
		  }
	  }

	}

}