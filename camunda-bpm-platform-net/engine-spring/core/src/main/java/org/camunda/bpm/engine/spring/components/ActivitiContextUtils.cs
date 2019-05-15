/*
 * Copyright 2010 the original author or authors
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
namespace org.camunda.bpm.engine.spring.components
{
	using StateHandlerAnnotationBeanFactoryPostProcessor = org.camunda.bpm.engine.spring.components.config.xml.StateHandlerAnnotationBeanFactoryPostProcessor;

	/// <summary>
	/// simple place to stash the constants used throughout the code
	/// 
	/// @author Josh Long
	/// @since 5.3
	/// </summary>
	public class ActivitiContextUtils
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		public static readonly string ANNOTATION_STATE_HANDLER_BEAN_FACTORY_POST_PROCESSOR_BEAN_NAME = typeof(StateHandlerAnnotationBeanFactoryPostProcessor).FullName.ToLower();
		/// <summary>
		/// the name of the default registry used to store all state handling components
		/// </summary>
		public const string ACTIVITI_REGISTRY_BEAN_NAME = "activitiComponentRegistry";


	}

}