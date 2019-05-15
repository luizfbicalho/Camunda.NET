﻿/*
 * Copyright 2011 the original author or authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
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
namespace org.camunda.bpm.engine.spring.annotations
{

	//todo 
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Target({ElementType.METHOD}) @Retention(RetentionPolicy.RUNTIME) @Documented public class StartTask extends System.Attribute
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class StartTask : System.Attribute
	{

		internal string userId;

		internal string taskId;

		/// <summary>
		/// the name of the task to start work on </summary>
		/// <returns> the name of the task </returns>
		internal string value;

		public StartTask(String taskId, String value, String userId = "")
		{
			this.userId = userId;
			this.taskId = taskId;
			this.value = value;
		}
	}

}